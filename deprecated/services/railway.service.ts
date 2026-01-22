import axios from 'axios';

export class RailwayService {
    private apiUrl: string;
    private apiToken: string;

    constructor() {
        this.apiUrl = process.env.RAILWAY_API_URL || 'https://api.railway.app/graphql/v2';
        this.apiToken = process.env.RAILWAY_TOKEN || '';
    }

    async getProjects() {
        try {
            const query = `
                query {
                    projects {
                        edges {
                            node {
                                id
                                name
                                description
                                createdAt
                                updatedAt
                                services {
                                    edges {
                                        node {
                                            id
                                            name
                                            status
                                            createdAt
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            `;

            const response = await axios.post(this.apiUrl, { query }, {
                headers: {
                    'Authorization': `Bearer ${this.apiToken}`,
                    'Content-Type': 'application/json'
                }
            });

            return {
                success: true,
                data: response.data.data?.projects?.edges || []
            };
        } catch (error: any) {
            console.error('Erro ao buscar projetos Railway:', error.message);
            return {
                success: false,
                error: error.message,
                data: []
            };
        }
    }

    async getProjectDetails(projectId: string) {
        try {
            const query = `
                query($projectId: String!) {
                    project(id: $projectId) {
                        id
                        name
                        description
                        createdAt
                        updatedAt
                        services {
                            edges {
                                node {
                                    id
                                    name
                                    status
                                    createdAt
                                    deployments {
                                        edges {
                                            node {
                                                id
                                                status
                                                createdAt
                                                staticUrl
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            `;

            const response = await axios.post(this.apiUrl, {
                query,
                variables: { projectId }
            }, {
                headers: {
                    'Authorization': `Bearer ${this.apiToken}`,
                    'Content-Type': 'application/json'
                }
            });

            return {
                success: true,
                data: response.data.data?.project
            };
        } catch (error: any) {
            console.error('Erro ao buscar detalhes do projeto Railway:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }

    async deployProject(projectId: string, environmentId?: string) {
        try {
            const mutation = `
                mutation($input: DeployInput!) {
                    deploy(input: $input) {
                        id
                        status
                        createdAt
                    }
                }
            `;

            const response = await axios.post(this.apiUrl, {
                query: mutation,
                variables: {
                    input: {
                        projectId,
                        environmentId
                    }
                }
            }, {
                headers: {
                    'Authorization': `Bearer ${this.apiToken}`,
                    'Content-Type': 'application/json'
                }
            });

            return {
                success: true,
                data: response.data.data?.deploy
            };
        } catch (error: any) {
            console.error('Erro ao fazer deploy no Railway:', error.message);
            return {
                success: false,
                error: error.message
            };
        }
    }
}

export const railwayService = new RailwayService();