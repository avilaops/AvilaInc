import axios from 'axios';

export class LinkedInService {
    private baseUrl: string;
    private accessToken: string;

    constructor() {
        this.baseUrl = 'https://api.linkedin.com/v2';
        this.accessToken = process.env.LINKEDIN_ACCESS_TOKEN || '';
    }

    private getAuthHeader() {
        return {
            'Authorization': `Bearer ${this.accessToken}`,
            'Content-Type': 'application/json',
            'X-Restli-Protocol-Version': '2.0.0'
        };
    }

    async getProfile() {
        try {
            const response = await axios.get(`${this.baseUrl}/people/~`, {
                headers: this.getAuthHeader(),
                params: {
                    projection: '(id,firstName,lastName,headline,vanityName,profilePicture(displayImage~:playableStreams))'
                }
            });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao buscar perfil LinkedIn:', error.message);
            return {
                success: false,
                error: error.message,
                data: null
            };
        }
    }

    async getConnections() {
        try {
            const response = await axios.get(`${this.baseUrl}/people/~/connections`, {
                headers: this.getAuthHeader(),
                params: {
                    projection: '(id,firstName,lastName,headline,vanityName,profilePicture(displayImage~:playableStreams))'
                }
            });

            return {
                success: true,
                data: response.data.elements || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar conexões LinkedIn:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getNetworkUpdates() {
        try {
            const response = await axios.get(`${this.baseUrl}/people/~/network/updates`, {
                headers: this.getAuthHeader(),
                params: {
                    type: 'SHAR',
                    count: 50
                }
            });

            return {
                success: true,
                data: response.data.elements || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar atualizações LinkedIn:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async postUpdate(text: string) {
        try {
            const response = await axios.post(`${this.baseUrl}/people/~/shares`, {
                owner: 'urn:li:person:~',
                text: {
                    text: text
                }
            }, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao postar atualização LinkedIn:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async searchPeople(keywords: string, count: number = 10) {
        try {
            const response = await axios.get(`${this.baseUrl}/people-search`, {
                headers: this.getAuthHeader(),
                params: {
                    keywords: keywords,
                    count: count,
                    projection: '(people:(id,firstName,lastName,headline,vanityName,profilePicture(displayImage~:playableStreams)))'
                }
            });

            return {
                success: true,
                data: response.data.people?.values || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar pessoas LinkedIn:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getCompanyUpdates(companyId: string) {
        try {
            const response = await axios.get(`${this.baseUrl}/companies/${companyId}/updates`, {
                headers: this.getAuthHeader(),
                params: {
                    count: 20
                }
            });

            return {
                success: true,
                data: response.data.values || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar atualizações da empresa LinkedIn:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async sendMessage(recipientId: string, message: string) {
        try {
            const response = await axios.post(`${this.baseUrl}/messages`, {
                recipients: {
                    values: [`urn:li:person:${recipientId}`]
                },
                body: message,
                subject: 'Mensagem via API'
            }, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao enviar mensagem LinkedIn:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }
}

export const linkedinService = new LinkedInService();