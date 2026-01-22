import axios from 'axios';

export class AzureDevOpsService {
    private baseUrl: string;
    private organization: string;
    private project: string;
    private token: string;

    constructor() {
        this.baseUrl = 'https://dev.azure.com';
        this.organization = process.env.AZURE_DEVOPS_ORG || '';
        this.project = process.env.AZURE_DEVOPS_PROJECT || '';
        this.token = process.env.AZURE_DEVOPS_TOKEN || '';
    }

    private getAuthHeader() {
        return {
            'Authorization': `Basic ${Buffer.from(`:${this.token}`).toString('base64')}`,
            'Content-Type': 'application/json'
        };
    }

    async getProjects() {
        try {
            const url = `${this.baseUrl}/${this.organization}/_apis/projects?api-version=6.0`;
            const response = await axios.get(url, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data.value || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar projetos Azure DevOps:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getBuilds(projectName?: string) {
        try {
            const project = projectName || this.project;
            const url = `${this.baseUrl}/${this.organization}/${project}/_apis/build/builds?api-version=6.0`;
            const response = await axios.get(url, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data.value || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar builds Azure DevOps:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getReleases(projectName?: string) {
        try {
            const project = projectName || this.project;
            const url = `${this.baseUrl}/${this.organization}/${project}/_apis/release/releases?api-version=6.0`;
            const response = await axios.get(url, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data.value || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar releases Azure DevOps:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getWorkItems(projectName?: string) {
        try {
            const project = projectName || this.project;
            const url = `${this.baseUrl}/${this.organization}/${project}/_apis/wit/workitems?api-version=6.0`;
            const response = await axios.get(url, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data.value || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar work items Azure DevOps:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async createBuild(projectName: string, buildDefinitionId: number, parameters?: any) {
        try {
            const url = `${this.baseUrl}/${this.organization}/${projectName}/_apis/build/builds?api-version=6.0`;
            const response = await axios.post(url, {
                definition: {
                    id: buildDefinitionId
                },
                parameters: JSON.stringify(parameters || {})
            }, { headers: this.getAuthHeader() });

            return {
                success: true,
                data: response.data
            };
        } catch (error: any) {
            console.error('Erro ao criar build Azure DevOps:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }
}

export const azureDevOpsService = new AzureDevOpsService();