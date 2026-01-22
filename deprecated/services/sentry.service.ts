import axios from 'axios';

export class SentryService {
    private baseUrl: string;
    private token: string;
    private organization: string;

    constructor() {
        this.baseUrl = 'https://sentry.io/api/0';
        this.token = process.env.SENTRY_TOKEN_API || '';
        this.organization = process.env.SENTRY_ORGANIZATION || '';
    }

    private getAuthHeader() {
        return {
            'Authorization': `Bearer ${this.token}`,
            'Content-Type': 'application/json'
        };
    }

    async getIssues(project?: string) {
        try {
            let url = `${this.baseUrl}/organizations/${this.organization}/issues/`;
            if (project) {
                url = `${this.baseUrl}/projects/${this.organization}/${project}/issues/`;
            }

            const response = await axios.get(url, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar issues Sentry:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getProjects() {
        try {
            const url = `${this.baseUrl}/organizations/${this.organization}/projects/`;
            const response = await axios.get(url, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar projetos Sentry:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getEvents(project: string, issueId: string) {
        try {
            const url = `${this.baseUrl}/projects/${this.organization}/${project}/issues/${issueId}/events/`;
            const response = await axios.get(url, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar eventos Sentry:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async resolveIssue(project: string, issueId: string) {
        try {
            const url = `${this.baseUrl}/projects/${this.organization}/${project}/issues/${issueId}/`;
            const response = await axios.put(url, {
                status: 'resolved'
            }, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao resolver issue Sentry:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async getStats(project?: string) {
        try {
            let url = `${this.baseUrl}/organizations/${this.organization}/stats/`;
            if (project) {
                url = `${this.baseUrl}/projects/${this.organization}/${project}/stats/`;
            }

            const response = await axios.get(url, {
                headers: this.getAuthHeader(),
                params: {
                    stat: 'received',
                    since: Math.floor(Date.now() / 1000) - (7 * 24 * 60 * 60), // 7 dias atrás
                    resolution: '1d'
                }
            });

            return {
                success: true,
                data: response.data || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar estatísticas Sentry:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }
}

export const sentryService = new SentryService();