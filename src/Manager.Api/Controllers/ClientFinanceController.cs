using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Manager.Core.Entities;

namespace Manager.Api.Controllers;

[ApiController]
[Route("api/client-finance")]
[Authorize]
public class ClientFinanceController : ControllerBase
{
    private readonly IMongoCollection<ClientFinance> _financeCollection;
    private readonly IMongoCollection<ClientContract> _contractsCollection;
    private readonly IMongoCollection<ClientHistory> _historyCollection;
    private readonly ILogger<ClientFinanceController> _logger;

    public ClientFinanceController(
        IMongoCollection<ClientFinance> financeCollection,
        IMongoCollection<ClientContract> contractsCollection,
        IMongoCollection<ClientHistory> historyCollection,
        ILogger<ClientFinanceController> logger)
    {
        _financeCollection = financeCollection;
        _contractsCollection = contractsCollection;
        _historyCollection = historyCollection;
        _logger = logger;
    }

    #region Finance Records

    /// <summary>
    /// Listar registros financeiros de um cliente
    /// </summary>
    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClientFinance(string clientId, [FromQuery] string? type = null)
    {
        try
        {
            var filterBuilder = Builders<ClientFinance>.Filter;
            var filter = filterBuilder.Eq(f => f.ClientId, clientId);

            if (!string.IsNullOrEmpty(type))
            {
                filter &= filterBuilder.Eq(f => f.Type, type);
            }

            var records = await _financeCollection
                .Find(filter)
                .SortByDescending(f => f.CreatedAt)
                .ToListAsync();

            var total = records.Sum(r => r.Amount);
            var pending = records.Where(r => r.Status == "pending").Sum(r => r.Amount);
            var paid = records.Where(r => r.Status == "paid").Sum(r => r.Amount);

            return Ok(new
            {
                success = true,
                clientId,
                count = records.Count,
                summary = new
                {
                    total,
                    pending,
                    paid,
                    currency = records.FirstOrDefault()?.Currency ?? "BRL"
                },
                data = records.Select(r => new
                {
                    id = r.Id,
                    type = r.Type,
                    amount = r.Amount,
                    currency = r.Currency,
                    status = r.Status,
                    paymentMethod = r.PaymentMethod,
                    dueDate = r.DueDate,
                    paidAt = r.PaidAt,
                    stripePaymentIntentId = r.StripePaymentIntentId,
                    createdAt = r.CreatedAt
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar finance records do cliente {ClientId}", clientId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Criar registro financeiro
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateFinanceRecord([FromBody] CreateFinanceRequest request)
    {
        try
        {
            var record = new ClientFinance
            {
                ClientId = request.ClientId,
                ClientName = request.ClientName,
                Type = request.Type,
                Amount = request.Amount,
                Currency = request.Currency ?? "BRL",
                Status = request.Status ?? "pending",
                PaymentMethod = request.PaymentMethod,
                DueDate = request.DueDate,
                StripePaymentIntentId = request.StripePaymentIntentId
            };

            await _financeCollection.InsertOneAsync(record);

            // Criar histórico
            await CreateHistoryEntry(
                request.ClientId,
                "finance_created",
                $"Registro financeiro criado: {request.Type} - {request.Amount:C}",
                User.Identity?.Name ?? "System"
            );

            _logger.LogInformation("Finance record criado: {Id} para {ClientId}", record.Id, request.ClientId);

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = record.Id,
                    clientId = record.ClientId,
                    amount = record.Amount,
                    status = record.Status
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar finance record");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar status de pagamento
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(string id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var record = await _financeCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
            if (record == null)
                return NotFound(new { success = false, message = "Registro não encontrado" });

            record.Status = request.Status;
            if (request.Status == "paid")
            {
                record.PaidAt = DateTime.UtcNow;
            }

            await _financeCollection.ReplaceOneAsync(f => f.Id == id, record);

            // Criar histórico
            await CreateHistoryEntry(
                record.ClientId,
                "payment_status_updated",
                $"Status atualizado para: {request.Status}",
                User.Identity?.Name ?? "System"
            );

            return Ok(new { success = true, data = record });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pagamento {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Deletar registro financeiro
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFinanceRecord(string id)
    {
        try
        {
            var record = await _financeCollection.Find(f => f.Id == id).FirstOrDefaultAsync();
            if (record == null)
                return NotFound(new { success = false, message = "Registro não encontrado" });

            await _financeCollection.DeleteOneAsync(f => f.Id == id);

            await CreateHistoryEntry(
                record.ClientId,
                "finance_deleted",
                $"Registro financeiro deletado: {record.Type} - {record.Amount:C}",
                User.Identity?.Name ?? "System"
            );

            return Ok(new { success = true, message = "Registro deletado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar finance record {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region Contracts

    /// <summary>
    /// Listar contratos de um cliente
    /// </summary>
    [HttpGet("{clientId}/contracts")]
    public async Task<IActionResult> GetClientContracts(string clientId)
    {
        try
        {
            var contracts = await _contractsCollection
                .Find(c => c.ClientId == clientId)
                .SortByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                clientId,
                count = contracts.Count,
                data = contracts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar contratos do cliente {ClientId}", clientId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Upload de contrato
    /// </summary>
    [HttpPost("{clientId}/contracts")]
    public async Task<IActionResult> CreateContract(string clientId, [FromBody] CreateContractRequest request)
    {
        try
        {
            var contract = new ClientContract
            {
                ClientId = clientId,
                ClientName = request.ClientName,
                ContractType = request.ContractType,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                FileSize = request.FileSize,
                Status = "pending",
                ExpiresAt = request.ExpiresAt
            };

            await _contractsCollection.InsertOneAsync(contract);

            await CreateHistoryEntry(
                clientId,
                "contract_uploaded",
                $"Contrato enviado: {request.FileName}",
                User.Identity?.Name ?? "System"
            );

            return Ok(new { success = true, data = contract });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar contrato");
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Atualizar status do contrato (assinar, expirar)
    /// </summary>
    [HttpPut("contracts/{id}/status")]
    public async Task<IActionResult> UpdateContractStatus(string id, [FromBody] UpdateContractStatusRequest request)
    {
        try
        {
            var contract = await _contractsCollection.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (contract == null)
                return NotFound(new { success = false, message = "Contrato não encontrado" });

            contract.Status = request.Status;
            if (request.Status == "signed")
            {
                contract.SignedAt = DateTime.UtcNow;
            }

            await _contractsCollection.ReplaceOneAsync(c => c.Id == id, contract);

            await CreateHistoryEntry(
                contract.ClientId,
                "contract_status_updated",
                $"Contrato {contract.FileName}: {request.Status}",
                User.Identity?.Name ?? "System"
            );

            return Ok(new { success = true, data = contract });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar contrato {Id}", id);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    #endregion

    #region History

    /// <summary>
    /// Obter histórico de um cliente
    /// </summary>
    [HttpGet("{clientId}/history")]
    public async Task<IActionResult> GetClientHistory(string clientId, [FromQuery] int limit = 50)
    {
        try
        {
            var history = await _historyCollection
                .Find(h => h.ClientId == clientId)
                .SortByDescending(h => h.CreatedAt)
                .Limit(limit)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                clientId,
                count = history.Count,
                data = history
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar histórico do cliente {ClientId}", clientId);
            return StatusCode(500, new { success = false, message = ex.Message });
        }
    }

    private async Task CreateHistoryEntry(string clientId, string action, string description, string performedBy)
    {
        var historyEntry = new ClientHistory
        {
            ClientId = clientId,
            Action = action,
            Description = description,
            PerformedBy = performedBy,
            Metadata = new Dictionary<string, string>
            {
                ["timestamp"] = DateTime.UtcNow.ToString("O")
            }
        };

        await _historyCollection.InsertOneAsync(historyEntry);
    }

    #endregion
}

#region DTOs

public record CreateFinanceRequest(
    string ClientId,
    string ClientName,
    string Type,
    decimal Amount,
    string? Currency = "BRL",
    string? Status = "pending",
    string? PaymentMethod = null,
    DateTime? DueDate = null,
    string? StripePaymentIntentId = null
);

public record UpdateStatusRequest(string Status);

public record CreateContractRequest(
    string ClientName,
    string ContractType,
    string FileName,
    string FileUrl,
    long FileSize,
    DateTime? ExpiresAt = null
);

public record UpdateContractStatusRequest(string Status);

#endregion
