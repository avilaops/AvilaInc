import axios from 'axios';

export class PayPalService {
    private baseUrl: string;
    private clientId: string;
    private clientSecret: string;
    private accessToken: string | null = null;
    private tokenExpiry: Date | null = null;

    constructor() {
        this.baseUrl = process.env.PAYPAL_ENVIRONMENT === 'sandbox'
            ? 'https://api-m.sandbox.paypal.com'
            : 'https://api-m.paypal.com';
        this.clientId = process.env.PAYPAL_CLIENT_ID || '';
        this.clientSecret = process.env.PAYPAL_CLIENT_SECRET || '';
    }

    private async getAccessToken() {
        try {
            if (this.accessToken && this.tokenExpiry && this.tokenExpiry > new Date()) {
                return this.accessToken;
            }

            const auth = Buffer.from(`${this.clientId}:${this.clientSecret}`).toString('base64');
            const response = await axios.post(`${this.baseUrl}/v1/oauth2/token`, 'grant_type=client_credentials', {
                headers: {
                    'Authorization': `Basic ${auth}`,
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            });

            this.accessToken = response.data.access_token;
            this.tokenExpiry = new Date(Date.now() + (response.data.expires_in * 1000));

            return this.accessToken;
        } catch (error: any) {
            console.error('Erro ao obter token PayPal:', error.message);
            throw error;
        }
    }

    private async getAuthHeader() {
        const token = await this.getAccessToken();
        return {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        };
    }

    async getBalance() {
        try {
            const headers = await this.getAuthHeader();
            const response = await axios.get(`${this.baseUrl}/v1/reporting/balances`, { headers });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao buscar saldo PayPal:', error.message);
            return {
                success: false,
                error: error.message,
                data: null
            };
        }
    }

    async getTransactions(startDate?: string, endDate?: string) {
        try {
            const headers = await this.getAuthHeader();
            const params: any = {};

            if (startDate) params.start_date = startDate;
            if (endDate) params.end_date = endDate;

            const response = await axios.get(`${this.baseUrl}/v1/reporting/transactions`, {
                headers,
                params
            });

            return {
                success: true,
                data: response.data.transaction_details || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar transações PayPal:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async createPayment(amount: number, currency: string = 'BRL', description?: string) {
        try {
            const headers = await this.getAuthHeader();
            const paymentData = {
                intent: 'sale',
                payer: {
                    payment_method: 'paypal'
                },
                transactions: [{
                    amount: {
                        total: amount.toFixed(2),
                        currency: currency
                    },
                    description: description || 'Pagamento via API'
                }],
                redirect_urls: {
                    return_url: process.env.PAYPAL_RETURN_URL || 'http://localhost:3000/payment/success',
                    cancel_url: process.env.PAYPAL_CANCEL_URL || 'http://localhost:3000/payment/cancel'
                }
            };

            const response = await axios.post(`${this.baseUrl}/v1/payments/payment`, paymentData, { headers });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao criar pagamento PayPal:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async executePayment(paymentId: string, payerId: string) {
        try {
            const headers = await this.getAuthHeader();
            const response = await axios.post(`${this.baseUrl}/v1/payments/payment/${paymentId}/execute`, {
                payer_id: payerId
            }, { headers });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao executar pagamento PayPal:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async refundPayment(saleId: string, amount?: number) {
        try {
            const headers = await this.getAuthHeader();
            const refundData: any = {};

            if (amount) {
                refundData.amount = {
                    total: amount.toFixed(2),
                    currency: 'BRL'
                };
            }

            const response = await axios.post(`${this.baseUrl}/v1/payments/sale/${saleId}/refund`, refundData, { headers });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao reembolsar pagamento PayPal:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }
}

export const paypalService = new PayPalService();