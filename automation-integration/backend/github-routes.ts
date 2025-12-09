import express, { Request, Response } from 'express';

const router = express.Router();

const REPO_OWNER = 'avilaops';
const REPO_NAME = 'AvilaInc';

router.get('/repository', async (req: Request, res: Response) => {
  try {
    const repoInfo = {
      name: REPO_NAME,
      full_name: `${REPO_OWNER}/${REPO_NAME}`,
      html_url: `https://github.com/${REPO_OWNER}/${REPO_NAME}`,
      description: 'Repositório do projeto AvilaInc',
      default_branch: 'main',
      updated_at: new Date().toISOString(),
      stargazers_count: 0,
      forks_count: 0,
      open_issues_count: 0
    };
    res.json(repoInfo);
  } catch (error) {
    res.status(500).json({ error: 'Erro ao buscar repositório' });
  }
});

router.get('/issues', async (req: Request, res: Response) => {
  try {
    res.json([]);
  } catch (error) {
    res.status(500).json({ error: 'Erro ao buscar issues' });
  }
});

router.post('/issues', async (req: Request, res: Response) => {
  try {
    const { title, body } = req.body;
    if (!title) {
      return res.status(400).json({ error: 'Título é obrigatório' });
    }
    res.json({
      number: Math.floor(Math.random() * 1000),
      title,
      html_url: `https://github.com/${REPO_OWNER}/${REPO_NAME}/issues/new`,
      state: 'open'
    });
  } catch (error) {
    res.status(500).json({ error: 'Erro ao criar issue' });
  }
});

router.get('/pulls', async (req: Request, res: Response) => {
  try {
    res.json([]);
  } catch (error) {
    res.status(500).json({ error: 'Erro ao buscar pull requests' });
  }
});

router.post('/pulls', async (req: Request, res: Response) => {
  try {
    const { title, head, base, body } = req.body;
    if (!title || !head || !base) {
      return res.status(400).json({ error: 'Título, head e base são obrigatórios' });
    }
    res.json({
      number: Math.floor(Math.random() * 1000),
      title,
      html_url: `https://github.com/${REPO_OWNER}/${REPO_NAME}/pulls`,
      state: 'open'
    });
  } catch (error) {
    res.status(500).json({ error: 'Erro ao criar pull request' });
  }
});

router.get('/branches', async (req: Request, res: Response) => {
  try {
    const branches = [
      { name: 'main', commit: { sha: 'abc123' }, protected: true },
      { name: 'development', commit: { sha: 'def456' }, protected: false }
    ];
    res.json(branches);
  } catch (error) {
    res.status(500).json({ error: 'Erro ao buscar branches' });
  }
});

export default router;