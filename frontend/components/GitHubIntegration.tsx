import React, { useState, useEffect } from 'react';

interface Repository {
  name: string;
  full_name: string;
  html_url: string;
  description: string;
  default_branch: string;
  updated_at: string;
}

interface Issue {
  number: number;
  title: string;
  state: string;
  html_url: string;
  created_at: string;
  user: {
    login: string;
  };
}

interface PullRequest {
  number: number;
  title: string;
  state: string;
  html_url: string;
  created_at: string;
  user: {
    login: string;
  };
}

interface Branch {
  name: string;
  commit: {
    sha: string;
  };
}

export const GitHubIntegration: React.FC = () => {
  const [activeSection, setActiveSection] = useState<'overview' | 'issues' | 'prs' | 'branches' | 'files'>('overview');
  const [loading, setLoading] = useState(false);
  const [repository, setRepository] = useState<Repository | null>(null);
  const [issues, setIssues] = useState<Issue[]>([]);
  const [pullRequests, setPullRequests] = useState<PullRequest[]>([]);
  const [branches, setBranches] = useState<Branch[]>([]);
  const [error, setError] = useState<string | null>(null);

  const REPO_OWNER = 'avilaops';
  const REPO_NAME = 'AvilaInc';

  useEffect(() => {
    loadRepositoryInfo();
  }, []);

  const loadRepositoryInfo = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`/api/github/repository`);
      if (!response.ok) throw new Error('Erro ao carregar repositório');
      const data = await response.json();
      setRepository(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido');
    } finally {
      setLoading(false);
    }
  };

  const loadIssues = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`/api/github/issues`);
      if (!response.ok) throw new Error('Erro ao carregar issues');
      const data = await response.json();
      setIssues(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido');
    } finally {
      setLoading(false);
    }
  };

  const loadPullRequests = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`/api/github/pulls`);
      if (!response.ok) throw new Error('Erro ao carregar pull requests');
      const data = await response.json();
      setPullRequests(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido');
    } finally {
      setLoading(false);
    }
  };

  const loadBranches = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`/api/github/branches`);
      if (!response.ok) throw new Error('Erro ao carregar branches');
      const data = await response.json();
      setBranches(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido');
    } finally {
      setLoading(false);
    }
  };

  const createIssue = async () => {
    const title = prompt('Título da issue:');
    if (!title) return;

    const body = prompt('Descrição da issue:');

    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`/api/github/issues`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, body })
      });
      if (!response.ok) throw new Error('Erro ao criar issue');
      await loadIssues();
      alert('Issue criada com sucesso!');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido');
    } finally {
      setLoading(false);
    }
  };

  const createPullRequest = async () => {
    const title = prompt('Título do Pull Request:');
    if (!title) return;

    const head = prompt('Branch de origem:');
    if (!head) return;

    const base = prompt('Branch de destino (ex: main):', 'main');
    if (!base) return;

    const body = prompt('Descrição do PR:');

    setLoading(true);
    setError(null);
    try {
      const response = await fetch(`/api/github/pulls`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, head, base, body })
      });
      if (!response.ok) throw new Error('Erro ao criar pull request');
      await loadPullRequests();
      alert('Pull Request criado com sucesso!');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro desconhecido');
    } finally {
      setLoading(false);
    }
  };  useEffect(() => {
    if (activeSection === 'issues' && issues.length === 0) {
      loadIssues();
    } else if (activeSection === 'prs' && pullRequests.length === 0) {
      loadPullRequests();
    } else if (activeSection === 'branches' && branches.length === 0) {
      loadBranches();
    }
  }, [activeSection]);

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-white rounded-lg shadow-lg p-6">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-2xl font-bold text-gray-900 flex items-center gap-2">
              <span className="text-3xl">🐙</span>
              Integração GitHub - AvilaInc
            </h2>
            <p className="text-gray-600 mt-2">
              Gerencie o repositório <strong>{REPO_OWNER}/{REPO_NAME}</strong> diretamente da plataforma
            </p>
          </div>
          <a
            href={`https://github.com/${REPO_OWNER}/${REPO_NAME}`}
            target="_blank"
            rel="noopener noreferrer"
            className="btn-primary"
          >
            🔗 Ver no GitHub
          </a>
        </div>

        {repository && (
          <div className="mt-4 grid grid-cols-3 gap-4">
            <div className="bg-purple-50 p-4 rounded-lg">
              <div className="text-sm text-purple-600 font-medium">Branch Padrão</div>
              <div className="text-lg font-bold text-purple-900">{repository.default_branch}</div>
            </div>
            <div className="bg-blue-50 p-4 rounded-lg">
              <div className="text-sm text-blue-600 font-medium">Última Atualização</div>
              <div className="text-lg font-bold text-blue-900">
                {new Date(repository.updated_at).toLocaleDateString('pt-BR')}
              </div>
            </div>
            <div className="bg-green-50 p-4 rounded-lg">
              <div className="text-sm text-green-600 font-medium">Status</div>
              <div className="text-lg font-bold text-green-900">✓ Ativo</div>
            </div>
          </div>
        )}
      </div>

      {/* Navigation Tabs */}
      <div className="bg-white rounded-lg shadow-lg overflow-hidden">
        <div className="flex border-b">
          <button
            onClick={() => setActiveSection('overview')}
            className={`px-6 py-3 font-medium transition-colors ${
              activeSection === 'overview'
                ? 'bg-purple-50 text-purple-700 border-b-2 border-purple-700'
                : 'text-gray-600 hover:bg-gray-50'
            }`}
          >
            📊 Visão Geral
          </button>
          <button
            onClick={() => setActiveSection('issues')}
            className={`px-6 py-3 font-medium transition-colors ${
              activeSection === 'issues'
                ? 'bg-purple-50 text-purple-700 border-b-2 border-purple-700'
                : 'text-gray-600 hover:bg-gray-50'
            }`}
          >
            🐛 Issues
          </button>
          <button
            onClick={() => setActiveSection('prs')}
            className={`px-6 py-3 font-medium transition-colors ${
              activeSection === 'prs'
                ? 'bg-purple-50 text-purple-700 border-b-2 border-purple-700'
                : 'text-gray-600 hover:bg-gray-50'
            }`}
          >
            🔀 Pull Requests
          </button>
          <button
            onClick={() => setActiveSection('branches')}
            className={`px-6 py-3 font-medium transition-colors ${
              activeSection === 'branches'
                ? 'bg-purple-50 text-purple-700 border-b-2 border-purple-700'
                : 'text-gray-600 hover:bg-gray-50'
            }`}
          >
            🌿 Branches
          </button>
          <button
            onClick={() => setActiveSection('files')}
            className={`px-6 py-3 font-medium transition-colors ${
              activeSection === 'files'
                ? 'bg-purple-50 text-purple-700 border-b-2 border-purple-700'
                : 'text-gray-600 hover:bg-gray-50'
            }`}
          >
            📁 Arquivos
          </button>
        </div>

        {/* Content */}
        <div className="p-6">
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              ⚠️ {error}
            </div>
          )}

          {loading && (
            <div className="text-center py-8">
              <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-purple-700"></div>
              <p className="mt-2 text-gray-600">Carregando...</p>
            </div>
          )}

          {!loading && activeSection === 'overview' && repository && (
            <div className="space-y-4">
              <div className="prose max-w-none">
                <h3 className="text-lg font-semibold">Sobre o Repositório</h3>
                <p className="text-gray-600">
                  {repository.description || 'Sem descrição disponível'}
                </p>
              </div>
              <div className="grid grid-cols-2 gap-4 mt-6">
                <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                  <h4 className="font-semibold text-gray-900 mb-2">🔧 Ferramentas Disponíveis</h4>
                  <ul className="text-sm text-gray-600 space-y-1">
                    <li>✓ Visualizar e criar issues</li>
                    <li>✓ Gerenciar pull requests</li>
                    <li>✓ Navegar por branches</li>
                    <li>✓ Explorar arquivos do código</li>
                    <li>✓ Buscar no repositório</li>
                  </ul>
                </div>
                <div className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                  <h4 className="font-semibold text-gray-900 mb-2">📈 Ações Rápidas</h4>
                  <div className="space-y-2">
                    <button onClick={createIssue} className="btn-secondary w-full text-sm">
                      ➕ Nova Issue
                    </button>
                    <button onClick={createPullRequest} className="btn-secondary w-full text-sm">
                      🔀 Novo Pull Request
                    </button>
                    <button onClick={loadRepositoryInfo} className="btn-secondary w-full text-sm">
                      🔄 Atualizar Dados
                    </button>
                  </div>
                </div>
              </div>
            </div>
          )}

          {!loading && activeSection === 'issues' && (
            <div className="space-y-4">
              <div className="flex justify-between items-center">
                <h3 className="text-lg font-semibold">Issues do Repositório</h3>
                <button onClick={createIssue} className="btn-primary text-sm">
                  ➕ Nova Issue
                </button>
              </div>
              {issues.length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  Nenhuma issue encontrada
                </div>
              ) : (
                <div className="space-y-3">
                  {issues.map((issue) => (
                    <div key={issue.number} className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <div className="flex items-center gap-2">
                            <span className={`px-2 py-1 text-xs rounded ${
                              issue.state === 'open' ? 'bg-green-100 text-green-800' : 'bg-purple-100 text-purple-800'
                            }`}>
                              {issue.state === 'open' ? '🟢 Aberta' : '🟣 Fechada'}
                            </span>
                            <span className="text-gray-500 text-sm">#{issue.number}</span>
                          </div>
                          <h4 className="font-semibold text-gray-900 mt-2">{issue.title}</h4>
                          <p className="text-sm text-gray-600 mt-1">
                            Criada por {issue.user.login} em {new Date(issue.created_at).toLocaleDateString('pt-BR')}
                          </p>
                        </div>
                        <a
                          href={issue.html_url}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="btn-secondary text-sm"
                        >
                          Ver no GitHub
                        </a>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {!loading && activeSection === 'prs' && (
            <div className="space-y-4">
              <div className="flex justify-between items-center">
                <h3 className="text-lg font-semibold">Pull Requests</h3>
                <button onClick={createPullRequest} className="btn-primary text-sm">
                  ➕ Novo Pull Request
                </button>
              </div>
              {pullRequests.length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  Nenhum pull request encontrado
                </div>
              ) : (
                <div className="space-y-3">
                  {pullRequests.map((pr) => (
                    <div key={pr.number} className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                      <div className="flex items-start justify-between">
                        <div className="flex-1">
                          <div className="flex items-center gap-2">
                            <span className={`px-2 py-1 text-xs rounded ${
                              pr.state === 'open' ? 'bg-green-100 text-green-800' : 'bg-purple-100 text-purple-800'
                            }`}>
                              {pr.state === 'open' ? '🟢 Aberto' : '🟣 Fechado'}
                            </span>
                            <span className="text-gray-500 text-sm">#{pr.number}</span>
                          </div>
                          <h4 className="font-semibold text-gray-900 mt-2">{pr.title}</h4>
                          <p className="text-sm text-gray-600 mt-1">
                            Criado por {pr.user.login} em {new Date(pr.created_at).toLocaleDateString('pt-BR')}
                          </p>
                        </div>
                        <a
                          href={pr.html_url}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="btn-secondary text-sm"
                        >
                          Ver no GitHub
                        </a>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {!loading && activeSection === 'branches' && (
            <div className="space-y-4">
              <h3 className="text-lg font-semibold">Branches do Repositório</h3>
              {branches.length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  Nenhuma branch encontrada
                </div>
              ) : (
                <div className="space-y-3">
                  {branches.map((branch) => (
                    <div key={branch.name} className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                      <div className="flex items-center justify-between">
                        <div>
                          <h4 className="font-semibold text-gray-900">{branch.name}</h4>
                          <p className="text-sm text-gray-600 mt-1">
                            Commit: {branch.commit.sha.substring(0, 7)}
                          </p>
                        </div>
                        <span className="text-2xl">🌿</span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {!loading && activeSection === 'files' && (
            <div className="space-y-4">
              <h3 className="text-lg font-semibold">Explorador de Arquivos</h3>
              <div className="text-center py-8 text-gray-500">
                <p className="mb-4">Funcionalidade em desenvolvimento</p>
                <p className="text-sm">
                  Em breve você poderá navegar pelos arquivos do repositório diretamente aqui.
                </p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
