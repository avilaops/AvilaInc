import express, { Express, Request, Response, NextFunction } from 'express';
import cors from 'cors';
import dotenv from 'dotenv';
import githubRoutes from './routes/github-routes';

dotenv.config();

const app: Express = express();
const port = process.env.PORT || 3001;

// Middlewares
app.use(cors({
  origin: process.env.FRONTEND_URL || 'http://localhost:3000',
  credentials: true,
}));

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// Request logging middleware
app.use((req: Request, res: Response, next: NextFunction) => {
  const start = Date.now();
  res.on('finish', () => {
    const duration = Date.now() - start;
    console.log(`${req.method} ${req.path} ${res.statusCode} - ${duration}ms`);
  });
  next();
});

// Routes
app.use('/api/github', githubRoutes);

// Health check endpoint
app.get('/health', (req: Request, res: Response) => {
  res.json({
    status: 'ok',
    service: 'automation-integration',
    timestamp: new Date().toISOString(),
    uptime: process.uptime(),
  });
});

// Root endpoint
app.get('/', (req: Request, res: Response) => {
  res.json({
    name: 'Avila Automation Integration API',
    version: '1.0.0',
    endpoints: {
      health: '/health',
      github: '/api/github',
    },
  });
});

// 404 handler
app.use((req: Request, res: Response) => {
  res.status(404).json({
    error: 'Not Found',
    message: `Route ${req.method} ${req.path} not found`,
  });
});

// Error handler
app.use((err: Error, req: Request, res: Response, next: NextFunction) => {
  console.error('Error:', err);
  res.status(500).json({
    error: 'Internal Server Error',
    message: err.message,
  });
});

// Start server
app.listen(port, () => {
  console.log(`
╔═══════════════════════════════════════════════════╗
║  🚀 Avila Automation Integration Server          ║
╠═══════════════════════════════════════════════════╣
║  Status: Running                                  ║
║  Port: ${port.toString().padEnd(44, ' ')}║
║  Environment: ${(process.env.NODE_ENV || 'development').padEnd(35, ' ')}║
║  GitHub Owner: ${(process.env.GITHUB_OWNER || 'avilaops').padEnd(33, ' ')}║
║  GitHub Repo: ${(process.env.GITHUB_REPO || 'AvilaInc').padEnd(34, ' ')}║
╠═══════════════════════════════════════════════════╣
║  Endpoints:                                       ║
║    GET  /health                                   ║
║    GET  /api/github/repository                    ║
║    GET  /api/github/issues                        ║
║    POST /api/github/issues                        ║
║    GET  /api/github/pulls                         ║
║    POST /api/github/pulls                         ║
║    GET  /api/github/branches                      ║
║    GET  /api/github/search/code                   ║
╚═══════════════════════════════════════════════════╝
  `);
});

export default app;
