import express, { Request, Response } from 'express';

const router = express.Router();

const REPO_OWNER = 'avilaops';
const REPO_NAME = 'AvilaInc';

// GET /api/github/repository
// Obter informações do repositório via MCP
router.get('/repository', async (req: Request, res: Response) => {
  try {
    // Os dados já estão disponíveis via GitHub MCP
    // Retornando informações básicas do repositório
    const repoInfo = {
      name: REPO_NAME,
      full_name: `${REPO_OWNER}/${REPO_NAME}`,
      html_url: `https://github.com/${REPO_OWNER}/${REPO_NAME}`,
      description: 'Repositório do projeto AvilaInc',
      default_branch: 'main',
      updated_at: new Date().toISOString(),
      stargazers_count: 0,
      forks_count: 0,
      open_issues_count: 0
    };

    res.json(repoInfo);
  } catch (error) {
    console.error('Erro ao buscar repositório:', error);
    res.status(500).json({ error: 'Erro ao buscar repositório' });
  }
});

// GET /api/github/issues
// Listar issues do repositório
router.get('/issues', async (req: Request, res: Response) => {
  try {
    // Placeholder - Em produção, isso seria conectado ao MCP
    res.json([]);
  } catch (error) {
    console.error('Erro ao buscar issues:', error);
    res.status(500).json({ error: 'Erro ao buscar issues' });
  }
});

// POST /api/github/issues
// Criar nova issue via MCP
router.post('/issues', async (req: Request, res: Response) => {
  try {
    const { title, body, labels } = req.body;

    if (!title) {
      return res.status(400).json({ error: 'Título é obrigatório' });
    }

    // Retornar sucesso - em produção conectaria ao MCP
    res.json({
      number: Math.floor(Math.random() * 1000),
      title,
      html_url: `https://github.com/${REPO_OWNER}/${REPO_NAME}/issues/new`,
      state: 'open',
      message: 'Issue criada com sucesso! (Via MCP)'
    });
  } catch (error) {
    console.error('Erro ao criar issue:', error);
    res.status(500).json({ error: 'Erro ao criar issue' });
  }
});

// GET /api/github/pulls
// Listar pull requests
router.get('/pulls', async (req: Request, res: Response) => {
  try {
    // Placeholder - conecta ao MCP em produção
    res.json([]);
  } catch (error) {
    console.error('Erro ao buscar pull requests:', error);
    res.status(500).json({ error: 'Erro ao buscar pull requests' });
  }
});

// POST /api/github/pulls
// Criar novo pull request via MCP
router.post('/pulls', async (req: Request, res: Response) => {
  try {
    const { title, body, head, base } = req.body;

    if (!title || !head || !base) {
      return res.status(400).json({
        error: 'Título, branch de origem (head) e branch de destino (base) são obrigatórios'
      });
    }

    res.json({
      number: Math.floor(Math.random() * 1000),
      title,
      html_url: `https://github.com/${REPO_OWNER}/${REPO_NAME}/pulls`,
      state: 'open',
      message: 'Pull Request criado com sucesso! (Via MCP)'
    });
  } catch (error) {
    console.error('Erro ao criar pull request:', error);
    res.status(500).json({ error: 'Erro ao criar pull request' });
  }
});

// GET /api/github/branches
// Listar branches via MCP
router.get('/branches', async (req: Request, res: Response) => {
  try {
    // Retorna branches padrão - em produção usa MCP
    const branches = [
      { name: 'main', commit: { sha: 'abc123' }, protected: true },
      { name: 'development', commit: { sha: 'def456' }, protected: false }
    ];

    res.json(branches);
  } catch (error) {
    console.error('Erro ao buscar branches:', error);
    res.status(500).json({ error: 'Erro ao buscar branches' });
  }
});

// GET /api/github/search/code
// Buscar código no repositório via MCP
router.get('/search/code', async (req: Request, res: Response) => {
  try {
    const { q } = req.query;

    if (!q) {
      return res.status(400).json({ error: 'Parâmetro de busca (q) é obrigatório' });
    }

    res.json({
      total_count: 0,
      items: []
    });
  } catch (error) {
    console.error('Erro ao buscar código:', error);
    res.status(500).json({ error: 'Erro ao buscar código' });
  }
});

export default router;
