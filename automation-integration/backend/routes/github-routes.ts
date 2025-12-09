import { Request, Response, Router } from 'express';
import axios from 'axios';

const router = Router();

// GitHub API configuration
const GITHUB_API = 'https://api.github.com';
const GITHUB_TOKEN = process.env.GITHUB_TOKEN;
const GITHUB_OWNER = process.env.GITHUB_OWNER || 'avilaops';
const GITHUB_REPO = process.env.GITHUB_REPO || 'AvilaInc';

// Axios instance for GitHub API
const githubApi = axios.create({
  baseURL: GITHUB_API,
  headers: {
    'Accept': 'application/vnd.github.v3+json',
    ...(GITHUB_TOKEN && { 'Authorization': `token ${GITHUB_TOKEN}` }),
  },
});

// Types
interface GitHubError {
  message: string;
  documentation_url?: string;
}

// Error handler
const handleGitHubError = (error: any, res: Response) => {
  console.error('GitHub API Error:', error.response?.data || error.message);

  if (error.response) {
    const githubError = error.response.data as GitHubError;
    res.status(error.response.status).json({
      error: githubError.message,
      details: githubError,
    });
  } else {
    res.status(500).json({
      error: 'Internal Server Error',
      message: error.message,
    });
  }
};

/**
 * GET /api/github/repository
 * Get repository information
 */
router.get('/repository', async (req: Request, res: Response) => {
  try {
    const { data } = await githubApi.get(`/repos/${GITHUB_OWNER}/${GITHUB_REPO}`);

    res.json({
      id: data.id,
      name: data.name,
      full_name: data.full_name,
      description: data.description,
      html_url: data.html_url,
      language: data.language,
      stargazers_count: data.stargazers_count,
      forks_count: data.forks_count,
      open_issues_count: data.open_issues_count,
      default_branch: data.default_branch,
      created_at: data.created_at,
      updated_at: data.updated_at,
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

/**
 * GET /api/github/issues
 * List repository issues
 * Query params: state (open/closed/all)
 */
router.get('/issues', async (req: Request, res: Response) => {
  try {
    const state = (req.query.state as string) || 'open';

    if (!['open', 'closed', 'all'].includes(state)) {
      return res.status(400).json({
        error: 'Invalid state parameter',
        message: 'State must be one of: open, closed, all',
      });
    }

    const { data } = await githubApi.get(
      `/repos/${GITHUB_OWNER}/${GITHUB_REPO}/issues`,
      {
        params: {
          state,
          per_page: 50,
        },
      }
    );

    // Filter out pull requests (GitHub API returns PRs as issues)
    const issues = data.filter((issue: any) => !issue.pull_request);

    res.json({
      count: issues.length,
      issues: issues.map((issue: any) => ({
        id: issue.id,
        number: issue.number,
        title: issue.title,
        state: issue.state,
        html_url: issue.html_url,
        user: {
          login: issue.user.login,
          avatar_url: issue.user.avatar_url,
        },
        labels: issue.labels.map((label: any) => ({
          name: label.name,
          color: label.color,
        })),
        created_at: issue.created_at,
        updated_at: issue.updated_at,
        body: issue.body,
      })),
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

/**
 * POST /api/github/issues
 * Create a new issue
 * Body: { title, body, labels? }
 */
router.post('/issues', async (req: Request, res: Response) => {
  try {
    const { title, body, labels } = req.body;

    if (!title) {
      return res.status(400).json({
        error: 'Validation Error',
        message: 'Title is required',
      });
    }

    const { data } = await githubApi.post(
      `/repos/${GITHUB_OWNER}/${GITHUB_REPO}/issues`,
      {
        title,
        body: body || '',
        labels: labels || [],
      }
    );

    res.status(201).json({
      id: data.id,
      number: data.number,
      title: data.title,
      state: data.state,
      html_url: data.html_url,
      created_at: data.created_at,
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

/**
 * GET /api/github/pulls
 * List pull requests
 * Query params: state (open/closed/all)
 */
router.get('/pulls', async (req: Request, res: Response) => {
  try {
    const state = (req.query.state as string) || 'open';

    if (!['open', 'closed', 'all'].includes(state)) {
      return res.status(400).json({
        error: 'Invalid state parameter',
        message: 'State must be one of: open, closed, all',
      });
    }

    const { data } = await githubApi.get(
      `/repos/${GITHUB_OWNER}/${GITHUB_REPO}/pulls`,
      {
        params: {
          state,
          per_page: 50,
        },
      }
    );

    res.json({
      count: data.length,
      pulls: data.map((pr: any) => ({
        id: pr.id,
        number: pr.number,
        title: pr.title,
        state: pr.state,
        html_url: pr.html_url,
        user: {
          login: pr.user.login,
          avatar_url: pr.user.avatar_url,
        },
        head: {
          ref: pr.head.ref,
          sha: pr.head.sha,
        },
        base: {
          ref: pr.base.ref,
        },
        merged: pr.merged,
        created_at: pr.created_at,
        updated_at: pr.updated_at,
      })),
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

/**
 * POST /api/github/pulls
 * Create a new pull request
 * Body: { title, head, base, body? }
 */
router.post('/pulls', async (req: Request, res: Response) => {
  try {
    const { title, head, base, body } = req.body;

    if (!title || !head || !base) {
      return res.status(400).json({
        error: 'Validation Error',
        message: 'Title, head, and base are required',
      });
    }

    const { data } = await githubApi.post(
      `/repos/${GITHUB_OWNER}/${GITHUB_REPO}/pulls`,
      {
        title,
        head,
        base,
        body: body || '',
      }
    );

    res.status(201).json({
      id: data.id,
      number: data.number,
      title: data.title,
      state: data.state,
      html_url: data.html_url,
      created_at: data.created_at,
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

/**
 * GET /api/github/branches
 * List all branches
 */
router.get('/branches', async (req: Request, res: Response) => {
  try {
    const { data } = await githubApi.get(
      `/repos/${GITHUB_OWNER}/${GITHUB_REPO}/branches`,
      {
        params: {
          per_page: 100,
        },
      }
    );

    res.json({
      count: data.length,
      branches: data.map((branch: any) => ({
        name: branch.name,
        commit: {
          sha: branch.commit.sha,
          url: branch.commit.url,
        },
        protected: branch.protected,
      })),
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

/**
 * GET /api/github/search/code
 * Search code in repository
 * Query params: q (search query)
 */
router.get('/search/code', async (req: Request, res: Response) => {
  try {
    const query = req.query.q as string;

    if (!query) {
      return res.status(400).json({
        error: 'Validation Error',
        message: 'Query parameter "q" is required',
      });
    }

    const searchQuery = `${query} repo:${GITHUB_OWNER}/${GITHUB_REPO}`;

    const { data } = await githubApi.get('/search/code', {
      params: {
        q: searchQuery,
        per_page: 30,
      },
    });

    res.json({
      total_count: data.total_count,
      items: data.items.map((item: any) => ({
        name: item.name,
        path: item.path,
        sha: item.sha,
        html_url: item.html_url,
        repository: {
          name: item.repository.name,
          full_name: item.repository.full_name,
        },
      })),
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

/**
 * GET /api/github/commits
 * List repository commits
 */
router.get('/commits', async (req: Request, res: Response) => {
  try {
    const { data } = await githubApi.get(
      `/repos/${GITHUB_OWNER}/${GITHUB_REPO}/commits`,
      {
        params: {
          per_page: 30,
        },
      }
    );

    res.json({
      count: data.length,
      commits: data.map((commit: any) => ({
        sha: commit.sha,
        message: commit.commit.message,
        author: {
          name: commit.commit.author.name,
          email: commit.commit.author.email,
          date: commit.commit.author.date,
        },
        html_url: commit.html_url,
      })),
    });
  } catch (error) {
    handleGitHubError(error, res);
  }
});

export default router;
