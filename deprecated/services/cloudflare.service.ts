import axios from 'axios';

export class CloudflareService {
    private baseUrl: string;
    private apiToken: string;
    private zoneId: string;

    constructor() {
        this.baseUrl = 'https://api.cloudflare.com/client/v4';
        this.apiToken = process.env.CLOUDFLARE_API_TOKEN || '';
        this.zoneId = process.env.CLOUDFLARE_ZONE_ID || '';
    }

    private getAuthHeader() {
        return {
            'Authorization': `Bearer ${this.apiToken}`,
            'Content-Type': 'application/json'
        };
    }

    async getZones() {
        try {
            const response = await axios.get(`${this.baseUrl}/zones`, {
                headers: this.getAuthHeader()
            });

            return {
                success: true,
                data: response.data.result || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar zonas Cloudflare:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getDNSRecords(zoneId?: string) {
        try {
            const zone = zoneId || this.zoneId;
            const response = await axios.get(`${this.baseUrl}/zones/${zone}/dns_records`, {
                headers: this.getAuthHeader()
            });

            return {
                success: true,
                data: response.data.result || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar registros DNS Cloudflare:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async createDNSRecord(record: {
        type: string;
        name: string;
        content: string;
        ttl?: number;
        proxied?: boolean;
    }, zoneId?: string) {
        try {
            const zone = zoneId || this.zoneId;
            const response = await axios.post(`${this.baseUrl}/zones/${zone}/dns_records`, {
                type: record.type,
                name: record.name,
                content: record.content,
                ttl: record.ttl || 1,
                proxied: record.proxied || false
            }, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data.result
            };
        } catch (error: any) {
            console.error('Erro ao criar registro DNS Cloudflare:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async updateDNSRecord(recordId: string, record: {
        type: string;
        name: string;
        content: string;
        ttl?: number;
        proxied?: boolean;
    }, zoneId?: string) {
        try {
            const zone = zoneId || this.zoneId;
            const response = await axios.put(`${this.baseUrl}/zones/${zone}/dns_records/${recordId}`, {
                type: record.type,
                name: record.name,
                content: record.content,
                ttl: record.ttl || 1,
                proxied: record.proxied || false
            }, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data.result
            };
        } catch (error: any) {
            console.error('Erro ao atualizar registro DNS Cloudflare:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async deleteDNSRecord(recordId: string, zoneId?: string) {
        try {
            const zone = zoneId || this.zoneId;
            const response = await axios.delete(`${this.baseUrl}/zones/${zone}/dns_records/${recordId}`, {
                headers: this.getAuthHeader()
            });

            return {
                success: true,
                data: response.data.result
            };
        } catch (error: any) {
            console.error('Erro ao deletar registro DNS Cloudflare:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async getAnalytics(zoneId?: string, since?: number, until?: number) {
        try {
            const zone = zoneId || this.zoneId;
            const params: any = {};

            if (since) params.since = since;
            if (until) params.until = until;

            const response = await axios.get(`${this.baseUrl}/zones/${zone}/analytics/dashboard`, {
                headers: this.getAuthHeader(),
                params
            });

            return {
                success: true,
                data: response.data.result || {}
            };
        } catch (error: any) {
            console.error('Erro ao buscar analytics Cloudflare:', error.message);
            return {
                success: false,
                error: error.message,
                data: {}
            };
        }
    }

    async purgeCache(zoneId?: string, files?: string[]) {
        try {
            const zone = zoneId || this.zoneId;
            const purgeData: any = {};

            if (files && files.length > 0) {
                purgeData.files = files;
            } else {
                purgeData.purge_everything = true;
            }

            const response = await axios.post(`${this.baseUrl}/zones/${zone}/purge_cache`, purgeData, {
                headers: this.getAuthHeader()
            });

            return {
                success: true,
                data: response.data.result
            };
        } catch (error: any) {
            console.error('Erro ao limpar cache Cloudflare:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async getPageRules(zoneId?: string) {
        try {
            const zone = zoneId || this.zoneId;
            const response = await axios.get(`${this.baseUrl}/zones/${zone}/pagerules`, {
                headers: this.getAuthHeader()
            });

            return {
                success: true,
                data: response.data.result || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar page rules Cloudflare:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }
}

export const cloudflareService = new CloudflareService();