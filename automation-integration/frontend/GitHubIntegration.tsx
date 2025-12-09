import React, { useState, useEffect } from 'react';
import './GitHubIntegration.css';

// Types
interface Repository {
  id: number;
  name: string;
  full_name: string;
  description: string | null;
  html_url: string;
  language: string | null;
  stargazers_count: number;
  forks_count: number;
  open_issues_count: number;
  default_branch: string;
  created_at: string;
  updated_at: string;
}

interface Issue {
  id: number;
  number: number;
  title: string;
  state: 'open' | 'closed';
  html_url: string;
  user: {
    login: string;
    avatar_url: string;
  };
  created_at: string;
  updated_at: string;
  labels: Array<{
    name: string;
    color: string;
  }>;
  assignees: Array<{
    login: string;
  }>;
}

interface PullRequest {
  id: number;
  number: number;
  title: string;
  state: 'open' | 'closed';
  html_url: string;
  user: {
    login: string;
    avatar_url: string;
  };
  created_at: string;
  updated_at: string;
  head: {
    ref: string;
  };
  base: {
    ref: string;
  };
}

interface Branch {
  name: string;
  commit: {
    sha: string;
  };
  protected: boolean;
}

interface FileContent {
  name: string;
  path: string;
  type: 'file' | 'dir';
  size: number;
  sha: string;
  download_url: string | null;
}

type TabType = 'overview' | 'issues' | 'pulls' | 'branches' | 'files';

const GitHubIntegration: React.FC = () => {
  const [activeTab, setActiveTab] = useState<TabType>('overview');
  const [repository, setRepository] = useState<Repository | null>(null);
  const [issues, setIssues] = useState<Issue[]>([]);
  const [pulls, setPulls] = useState<PullRequest[]>([]);
  const [branches, setBranches] = useState<Branch[]>([]);
  const [files, setFiles] = useState<FileContent[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:3001';

  // Fetch repository data
  useEffect(() => {
    fetchRepository();
  }, []);

  // Fetch data based on active tab
  useEffect(() => {
    switch (activeTab) {
      case 'issues':
        fetchIssues();
        break;
      case 'pulls':
        fetchPulls();
        break;
      case 'branches':
        fetchBranches();
        break;
      case 'files':
        fetchFiles();
        break;
    }
  }, [activeTab]);

  const fetchRepository = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(`${API_BASE}/api/github/repository`);
      if (!response.ok) throw new Error('Failed to fetch repository');
      const data = await response.json();
      setRepository(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const fetchIssues = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(`${API_BASE}/api/github/issues?state=all`);
      if (!response.ok) throw new Error('Failed to fetch issues');
      const data = await response.json();
      setIssues(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const fetchPulls = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(`${API_BASE}/api/github/pulls?state=all`);
      if (!response.ok) throw new Error('Failed to fetch pull requests');
      const data = await response.json();
      setPulls(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const fetchBranches = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(`${API_BASE}/api/github/branches`);
      if (!response.ok) throw new Error('Failed to fetch branches');
      const data = await response.json();
      setBranches(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const fetchFiles = async (path: string = '') => {
    try {
      setLoading(true);
      setError(null);
      const response = await fetch(`${API_BASE}/api/github/files?path=${path}`);
      if (!response.ok) throw new Error('Failed to fetch files');
      const data = await response.json();
      setFiles(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  return (
    <div className="github-integration">
      {/* Header */}
      <div className="github-header">
        <h1>🔗 Integração GitHub</h1>
        {repository && (
          <div className="repo-info">
            <a href={repository.html_url} target="_blank" rel="noopener noreferrer">
              {repository.full_name}
            </a>
          </div>
        )}
      </div>

      {/* Tabs Navigation */}
      <div className="tabs-navigation">
        <button
          className={`tab-button ${activeTab === 'overview' ? 'active' : ''}`}
          onClick={() => setActiveTab('overview')}
        >
          📊 Visão Geral
        </button>
        <button
          className={`tab-button ${activeTab === 'issues' ? 'active' : ''}`}
          onClick={() => setActiveTab('issues')}
        >
          🐛 Issues
        </button>
        <button
          className={`tab-button ${activeTab === 'pulls' ? 'active' : ''}`}
          onClick={() => setActiveTab('pulls')}
        >
          🔀 Pull Requests
        </button>
        <button
          className={`tab-button ${activeTab === 'branches' ? 'active' : ''}`}
          onClick={() => setActiveTab('branches')}
        >
          🌿 Branches
        </button>
        <button
          className={`tab-button ${activeTab === 'files' ? 'active' : ''}`}
          onClick={() => setActiveTab('files')}
        >
          📁 Arquivos
        </button>
      </div>

      {/* Error Message */}
      {error && (
        <div className="error-message">
          ⚠️ {error}
        </div>
      )}

      {/* Loading */}
      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          <p>Carregando...</p>
        </div>
      )}

      {/* Tab Content */}
      {!loading && (
        <div className="tab-content">
          {/* Overview Tab */}
          {activeTab === 'overview' && repository && (
            <div className="overview-content">
              <div className="stat-card">
                <h3>⭐ Stars</h3>
                <p className="stat-value">{repository.stargazers_count}</p>
              </div>
              <div className="stat-card">
                <h3>🔀 Forks</h3>
                <p className="stat-value">{repository.forks_count}</p>
              </div>
              <div className="stat-card">
                <h3>🐛 Issues Abertas</h3>
                <p className="stat-value">{repository.open_issues_count}</p>
              </div>
              <div className="stat-card">
                <h3>💻 Linguagem</h3>
                <p className="stat-value">{repository.language || 'N/A'}</p>
              </div>
              <div className="stat-card full-width">
                <h3>📝 Descrição</h3>
                <p>{repository.description || 'Sem descrição'}</p>
              </div>
              <div className="stat-card">
                <h3>📅 Criado em</h3>
                <p>{formatDate(repository.created_at)}</p>
              </div>
              <div className="stat-card">
                <h3>🔄 Última atualização</h3>
                <p>{formatDate(repository.updated_at)}</p>
              </div>
            </div>
          )}

          {/* Issues Tab */}
          {activeTab === 'issues' && (
            <div className="list-content">
              <div className="list-header">
                <h2>Issues ({issues.length})</h2>
              </div>
              {issues.length === 0 ? (
                <p className="empty-state">Nenhuma issue encontrada</p>
              ) : (
                <div className="list-items">
                  {issues.map((issue) => (
                    <div key={issue.id} className="list-item">
                      <div className="item-header">
                        <span className={`status-badge ${issue.state}`}>
                          {issue.state === 'open' ? '🟢' : '🔴'} {issue.state}
                        </span>
                        <a href={issue.html_url} target="_blank" rel="noopener noreferrer">
                          #{issue.number}
                        </a>
                      </div>
                      <h3>{issue.title}</h3>
                      <div className="item-meta">
                        <span>👤 {issue.user.login}</span>
                        <span>📅 {formatDate(issue.created_at)}</span>
                      </div>
                      {issue.labels.length > 0 && (
                        <div className="labels">
                          {issue.labels.map((label) => (
                            <span
                              key={label.name}
                              className="label"
                              style={{ backgroundColor: `#${label.color}` }}
                            >
                              {label.name}
                            </span>
                          ))}
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* Pull Requests Tab */}
          {activeTab === 'pulls' && (
            <div className="list-content">
              <div className="list-header">
                <h2>Pull Requests ({pulls.length})</h2>
              </div>
              {pulls.length === 0 ? (
                <p className="empty-state">Nenhum pull request encontrado</p>
              ) : (
                <div className="list-items">
                  {pulls.map((pr) => (
                    <div key={pr.id} className="list-item">
                      <div className="item-header">
                        <span className={`status-badge ${pr.state}`}>
                          {pr.state === 'open' ? '🟢' : '🟣'} {pr.state}
                        </span>
                        <a href={pr.html_url} target="_blank" rel="noopener noreferrer">
                          #{pr.number}
                        </a>
                      </div>
                      <h3>{pr.title}</h3>
                      <div className="item-meta">
                        <span>👤 {pr.user.login}</span>
                        <span>🌿 {pr.head.ref} → {pr.base.ref}</span>
                        <span>📅 {formatDate(pr.created_at)}</span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* Branches Tab */}
          {activeTab === 'branches' && (
            <div className="list-content">
              <div className="list-header">
                <h2>Branches ({branches.length})</h2>
              </div>
              {branches.length === 0 ? (
                <p className="empty-state">Nenhuma branch encontrada</p>
              ) : (
                <div className="list-items">
                  {branches.map((branch) => (
                    <div key={branch.name} className="list-item">
                      <div className="item-header">
                        <h3>🌿 {branch.name}</h3>
                        {branch.protected && (
                          <span className="protected-badge">🔒 Protegida</span>
                        )}
                      </div>
                      <div className="item-meta">
                        <span>📝 {branch.commit.sha.substring(0, 7)}</span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* Files Tab */}
          {activeTab === 'files' && (
            <div className="list-content">
              <div className="list-header">
                <h2>Arquivos ({files.length})</h2>
              </div>
              {files.length === 0 ? (
                <p className="empty-state">Nenhum arquivo encontrado</p>
              ) : (
                <div className="list-items">
                  {files.map((file) => (
                    <div key={file.path} className="list-item">
                      <div className="item-header">
                        <h3>
                          {file.type === 'dir' ? '📁' : '📄'} {file.name}
                        </h3>
                      </div>
                      <div className="item-meta">
                        <span>📊 {file.size} bytes</span>
                        <span>🔑 {file.sha.substring(0, 7)}</span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default GitHubIntegration;
