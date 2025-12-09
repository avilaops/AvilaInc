use std::io::{Read, Write};
use std::net::TcpStream;

#[derive(Clone)]
pub struct GitHubClient {
    owner: String,
    repo: String,
    token: Option<String>,
}

impl GitHubClient {
    pub fn new(owner: String, repo: String, token: Option<String>) -> Self {
        Self { owner, repo, token }
    }

    pub fn get_owner(&self) -> &str {
        &self.owner
    }

    pub fn get_repo(&self) -> &str {
        &self.repo
    }

    /// Faz requisição HTTP GET para a API do GitHub
    pub fn get(&self, path: &str) -> Result<String, Box<dyn std::error::Error>> {
        let mut stream = TcpStream::connect("api.github.com:443")?;
        
        // Configura TLS (simplificado - em produção usar biblioteca TLS)
        let request = format!(
            "GET {} HTTP/1.1\r\n\
             Host: api.github.com\r\n\
             User-Agent: Avila-Integration/1.0\r\n\
             Accept: application/vnd.github.v3+json\r\n\
             {}\
             Connection: close\r\n\
             \r\n",
            path,
            self.auth_header()
        );

        stream.write_all(request.as_bytes())?;

        let mut response = String::new();
        stream.read_to_string(&mut response)?;

        // Parse response (simplificado)
        if let Some(body_start) = response.find("\r\n\r\n") {
            Ok(response[body_start + 4..].to_string())
        } else {
            Ok(response)
        }
    }

    /// Faz requisição HTTP POST para a API do GitHub
    pub fn post(&self, path: &str, body: &str) -> Result<String, Box<dyn std::error::Error>> {
        let mut stream = TcpStream::connect("api.github.com:443")?;
        
        let request = format!(
            "POST {} HTTP/1.1\r\n\
             Host: api.github.com\r\n\
             User-Agent: Avila-Integration/1.0\r\n\
             Accept: application/vnd.github.v3+json\r\n\
             Content-Type: application/json\r\n\
             Content-Length: {}\r\n\
             {}\
             Connection: close\r\n\
             \r\n\
             {}",
            path,
            body.len(),
            self.auth_header(),
            body
        );

        stream.write_all(request.as_bytes())?;

        let mut response = String::new();
        stream.read_to_string(&mut response)?;

        if let Some(body_start) = response.find("\r\n\r\n") {
            Ok(response[body_start + 4..].to_string())
        } else {
            Ok(response)
        }
    }

    fn auth_header(&self) -> String {
        if let Some(ref token) = self.token {
            format!("Authorization: token {}\r\n", token)
        } else {
            String::new()
        }
    }

    // API Methods
    pub fn get_repository(&self) -> Result<String, Box<dyn std::error::Error>> {
        self.get(&format!("/repos/{}/{}", self.owner, self.repo))
    }

    pub fn list_issues(&self, state: Option<&str>) -> Result<String, Box<dyn std::error::Error>> {
        let state_param = state.unwrap_or("open");
        self.get(&format!("/repos/{}/{}/issues?state={}", self.owner, self.repo, state_param))
    }

    pub fn list_pulls(&self, state: Option<&str>) -> Result<String, Box<dyn std::error::Error>> {
        let state_param = state.unwrap_or("open");
        self.get(&format!("/repos/{}/{}/pulls?state={}", self.owner, self.repo, state_param))
    }

    pub fn list_branches(&self) -> Result<String, Box<dyn std::error::Error>> {
        self.get(&format!("/repos/{}/{}/branches", self.owner, self.repo))
    }

    pub fn list_commits(&self) -> Result<String, Box<dyn std::error::Error>> {
        self.get(&format!("/repos/{}/{}/commits?per_page=30", self.owner, self.repo))
    }
}
