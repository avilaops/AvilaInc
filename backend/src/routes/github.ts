import express, { Request, Response } from 'express';
import { Octokit } from '@octokit/rest';

const router = express.Router();

// Inicializar Octokit (cliente GitHub)
// Nota: Em produção, use uma variável de ambiente para o token
const octokit = new Octokit({
  auth: process.env.GITHUB_TOKEN
});

// GET /api/github/repository/:owner/:repo
// Obter informações do repositório
router.get('/repository/:owner/:repo', async (req: Request, res: Response) => {
  try {
    const { owner, repo } = req.params;

    const { data } = await octokit.repos.get({
      owner,
      repo
    });

    res.json({
      name: data.name,
      full_name: data.full_name,
      html_url: data.html_url,
      description: data.description,
      default_branch: data.default_branch,
      updated_at: data.updated_at,
      created_at: data.created_at,
      stargazers_count: data.stargazers_count,
      forks_count: data.forks_count,
      open_issues_count: data.open_issues_count
    });
  } catch (error) {
    console.error('Erro ao buscar repositório:', error);
    res.status(500).json({ error: 'Erro ao buscar repositório' });
  }
});

// GET /api/github/issues/:owner/:repo
// Listar issues do repositório
router.get('/issues/:owner/:repo', async (req, res) => {
  try {
    const { owner, repo } = req.params;
    const { state = 'all', per_page = 30 } = req.query;

    const { data } = await octokit.issues.listForRepo({
      owner,
      repo,
      state: state as any,
      per_page: Number(per_page)
    });

    res.json(data.map(issue => ({
      number: issue.number,
      title: issue.title,
      state: issue.state,
      html_url: issue.html_url,
      created_at: issue.created_at,
      updated_at: issue.updated_at,
      user: {
        login: issue.user?.login,
        avatar_url: issue.user?.avatar_url
      },
      labels: issue.labels
    })));
  } catch (error) {
    console.error('Erro ao buscar issues:', error);
    res.status(500).json({ error: 'Erro ao buscar issues' });
  }
});

// POST /api/github/issues/:owner/:repo
// Criar nova issue
router.post('/issues/:owner/:repo', async (req, res) => {
  try {
    const { owner, repo } = req.params;
    const { title, body, labels } = req.body;

    if (!title) {
      return res.status(400).json({ error: 'Título é obrigatório' });
    }

    const { data } = await octokit.issues.create({
      owner,
      repo,
      title,
      body,
      labels
    });

    res.json({
      number: data.number,
      title: data.title,
      html_url: data.html_url,
      state: data.state
    });
  } catch (error) {
    console.error('Erro ao criar issue:', error);
    res.status(500).json({ error: 'Erro ao criar issue' });
  }
});

// GET /api/github/pulls/:owner/:repo
// Listar pull requests
router.get('/pulls/:owner/:repo', async (req, res) => {
  try {
    const { owner, repo } = req.params;
    const { state = 'all', per_page = 30 } = req.query;

    const { data } = await octokit.pulls.list({
      owner,
      repo,
      state: state as any,
      per_page: Number(per_page)
    });

    res.json(data.map(pr => ({
      number: pr.number,
      title: pr.title,
      state: pr.state,
      html_url: pr.html_url,
      created_at: pr.created_at,
      updated_at: pr.updated_at,
      user: {
        login: pr.user?.login,
        avatar_url: pr.user?.avatar_url
      },
      head: {
        ref: pr.head.ref
      },
      base: {
        ref: pr.base.ref
      }
    })));
  } catch (error) {
    console.error('Erro ao buscar pull requests:', error);
    res.status(500).json({ error: 'Erro ao buscar pull requests' });
  }
});

// POST /api/github/pulls/:owner/:repo
// Criar novo pull request
router.post('/pulls/:owner/:repo', async (req, res) => {
  try {
    const { owner, repo } = req.params;
    const { title, body, head, base } = req.body;

    if (!title || !head || !base) {
      return res.status(400).json({
        error: 'Título, branch de origem (head) e branch de destino (base) são obrigatórios'
      });
    }

    const { data } = await octokit.pulls.create({
      owner,
      repo,
      title,
      body,
      head,
      base
    });

    res.json({
      number: data.number,
      title: data.title,
      html_url: data.html_url,
      state: data.state
    });
  } catch (error) {
    console.error('Erro ao criar pull request:', error);
    res.status(500).json({ error: 'Erro ao criar pull request' });
  }
});

// GET /api/github/branches/:owner/:repo
// Listar branches
router.get('/branches/:owner/:repo', async (req, res) => {
  try {
    const { owner, repo } = req.params;
    const { per_page = 30 } = req.query;

    const { data } = await octokit.repos.listBranches({
      owner,
      repo,
      per_page: Number(per_page)
    });

    res.json(data.map(branch => ({
      name: branch.name,
      commit: {
        sha: branch.commit.sha
      },
      protected: branch.protected
    })));
  } catch (error) {
    console.error('Erro ao buscar branches:', error);
    res.status(500).json({ error: 'Erro ao buscar branches' });
  }
});

// GET /api/github/contents/:owner/:repo/*
// Obter conteúdo de um arquivo ou diretório
router.get('/contents/:owner/:repo/*', async (req, res) => {
  try {
    const { owner, repo } = req.params;
    const path = req.params[0] || '';

    const { data } = await octokit.repos.getContent({
      owner,
      repo,
      path
    });

    res.json(data);
  } catch (error) {
    console.error('Erro ao buscar conteúdo:', error);
    res.status(500).json({ error: 'Erro ao buscar conteúdo' });
  }
});

// GET /api/github/search/code/:owner/:repo
// Buscar código no repositório
router.get('/search/code/:owner/:repo', async (req, res) => {
  try {
    const { owner, repo } = req.params;
    const { q, per_page = 30 } = req.query;

    if (!q) {
      return res.status(400).json({ error: 'Parâmetro de busca (q) é obrigatório' });
    }

    const { data } = await octokit.search.code({
      q: `${q} repo:${owner}/${repo}`,
      per_page: Number(per_page)
    });

    res.json({
      total_count: data.total_count,
      items: data.items.map(item => ({
        name: item.name,
        path: item.path,
        html_url: item.html_url,
        repository: {
          full_name: item.repository.full_name
        }
      }))
    });
  } catch (error) {
    console.error('Erro ao buscar código:', error);
    res.status(500).json({ error: 'Erro ao buscar código' });
  }
});

export default router;
