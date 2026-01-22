import axios from 'axios';

export class DNSService {
    private baseUrl: string;
    private apiKey: string;
    private secretKey: string;

    constructor() {
        this.baseUrl = 'https://api.porkbun.com/api/json/v3';
        this.apiKey = process.env.PORKBUN_API_KEY || '';
        this.secretKey = process.env.PORKBUN_SECRET_KEY || '';
    }

    private getAuthData() {
        return {
            apikey: this.apiKey,
            secretapikey: this.secretKey
        };
    }

    async getDomains() {
        try {
            const response = await axios.post(`${this.baseUrl}/domain/listAll`, this.getAuthData());

            return {
                success: true,
                data: response.data.domains || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar domínios Porkbun:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getDomainRecords(domain: string) {
        try {
            const response = await axios.post(`${this.baseUrl}/dns/retrieve/${domain}`, this.getAuthData());

            return {
                success: true,
                data: response.data.records || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar registros DNS:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async createRecord(domain: string, record: {
        name: string;
        type: string;
        content: string;
        ttl?: number;
        prio?: number;
    }) {
        try {
            const recordData = {
                ...this.getAuthData(),
                name: record.name,
                type: record.type,
                content: record.content,
                ttl: record.ttl || 600,
                ...(record.prio && { prio: record.prio })
            };

            const response = await axios.post(`${this.baseUrl}/dns/create/${domain}`, recordData);

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao criar registro DNS:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async updateRecord(domain: string, recordId: string, record: {
        name: string;
        type: string;
        content: string;
        ttl?: number;
        prio?: number;
    }) {
        try {
            const recordData = {
                ...this.getAuthData(),
                name: record.name,
                type: record.type,
                content: record.content,
                ttl: record.ttl || 600,
                ...(record.prio && { prio: record.prio })
            };

            const response = await axios.post(`${this.baseUrl}/dns/edit/${domain}/${recordId}`, recordData);

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao atualizar registro DNS:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async deleteRecord(domain: string, recordId: string) {
        try {
            const response = await axios.post(`${this.baseUrl}/dns/delete/${domain}/${recordId}`, this.getAuthData());

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao deletar registro DNS:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async getDomainPricing() {
        try {
            const response = await axios.post(`${this.baseUrl}/domain/getPricing`, this.getAuthData());

            return {
                success: true,
                data: response.data.pricing || {}
            };
        } catch (error: any) {
            console.error('Erro ao buscar preços de domínio:', error.message);
            return {
                success: false,
                error: error.message,
                data: {}
            };
        }
    }
}

export const dnsService = new DNSService();