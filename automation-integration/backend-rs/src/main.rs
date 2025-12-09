use std::sync::Arc;
use std::thread;
use serde::Serialize;

mod github;
use github::GitHubClient;

#[derive(Serialize)]
struct HealthResponse {
    status: String,
    service: String,
    timestamp: String,
}

#[derive(Serialize)]
struct ApiInfo {
    name: String,
    version: String,
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let port = std::env::var("PORT")
        .unwrap_or_else(|_| "3005".to_string())
        .parse::<u16>()
        .unwrap_or(3005);

    let github_owner = std::env::var("GITHUB_OWNER")
        .unwrap_or_else(|_| "avilaops".to_string());

    let github_repo = std::env::var("GITHUB_REPO")
        .unwrap_or_else(|_| "AvilaInc".to_string());

    let github_token = std::env::var("GITHUB_TOKEN").ok();

    let github_client = Arc::new(GitHubClient::new(
        github_owner.clone(),
        github_repo.clone(),
        github_token,
    ));

    print_banner(port, &github_owner, &github_repo);

    let listener = std::net::TcpListener::bind(format!("0.0.0.0:{}", port))?;
    println!("✅ Server listening on port {}", port);

    for stream in listener.incoming() {
        match stream {
            Ok(stream) => {
                let client = Arc::clone(&github_client);
                thread::spawn(move || {
                    if let Err(e) = handle_connection(stream, client) {
                        eprintln!("❌ Error: {}", e);
                    }
                });
            }
            Err(e) => eprintln!("❌ Connection error: {}", e),
        }
    }

    Ok(())
}

fn handle_connection(
    mut stream: std::net::TcpStream,
    client: Arc<GitHubClient>,
) -> Result<(), Box<dyn std::error::Error>> {
    use std::io::{Read, Write};

    let mut buffer = [0u8; 2048];
    let n = stream.read(&mut buffer)?;
    let request = String::from_utf8_lossy(&buffer[..n]);
    let lines: Vec<&str> = request.lines().collect();

    if lines.is_empty() {
        return Ok(());
    }

    let parts: Vec<&str> = lines[0].split_whitespace().collect();
    if parts.len() < 2 {
        return Ok(());
    }

    let (method, path) = (parts[0], parts[1]);
    println!("📨 {} {}", method, path);

    let response = match (method, path) {
        ("GET", "/") => {
            let body = serde_json::to_string(&ApiInfo {
                name: "Avila Automation Integration API".to_string(),
                version: "1.0.0".to_string(),
            })?;
            http_response(200, &body)
        }
        ("GET", "/health") => {
            let body = serde_json::to_string(&HealthResponse {
                status: "ok".to_string(),
                service: "automation-integration".to_string(),
                timestamp: format!("{:?}", std::time::SystemTime::now()),
            })?;
            http_response(200, &body)
        }
        ("GET", "/api/github/repository") => {
            match client.get_repository() {
                Ok(data) => http_response(200, &data),
                Err(e) => error_response(&format!("GitHub API error: {}", e)),
            }
        }
        ("GET", "/api/github/issues") => {
            match client.list_issues(None) {
                Ok(data) => http_response(200, &data),
                Err(e) => error_response(&format!("GitHub API error: {}", e)),
            }
        }
        ("GET", "/api/github/pulls") => {
            match client.list_pulls(None) {
                Ok(data) => http_response(200, &data),
                Err(e) => error_response(&format!("GitHub API error: {}", e)),
            }
        }
        ("GET", "/api/github/branches") => {
            match client.list_branches() {
                Ok(data) => http_response(200, &data),
                Err(e) => error_response(&format!("GitHub API error: {}", e)),
            }
        }
        ("GET", "/api/github/commits") => {
            match client.list_commits() {
                Ok(data) => http_response(200, &data),
                Err(e) => error_response(&format!("GitHub API error: {}", e)),
            }
        }
        ("GET", path) if path.starts_with("/api/github") => {
            let body = format!(r#"{{"message":"Endpoint not implemented","path":"{}"}}"#, path);
            http_response(501, &body)
        }
        _ => {
            let body = format!(r#"{{"error":"Not Found","path":"{}"}}"#, path);
            http_response(404, &body)
        }
    };

    stream.write_all(response.as_bytes())?;
    stream.flush()?;
    Ok(())
}

fn http_response(status: u16, body: &str) -> String {
    let status_text = match status {
        200 => "OK",
        404 => "NOT FOUND",
        500 => "INTERNAL SERVER ERROR",
        501 => "NOT IMPLEMENTED",
        _ => "UNKNOWN",
    };
    
    format!(
        "HTTP/1.1 {} {}\r\nContent-Type: application/json\r\nAccess-Control-Allow-Origin: *\r\nContent-Length: {}\r\n\r\n{}",
        status, status_text, body.len(), body
    )
}

fn error_response(error: &str) -> String {
    let body = format!(r#"{{"error":"{}"}}"#, error.replace('"', "\\\""));
    http_response(500, &body)
}

fn print_banner(port: u16, owner: &str, repo: &str) {
    println!("\n╔═══════════════════════════════════════════════════╗");
    println!("║  🚀 Avila Automation Integration Server          ║");
    println!("╠═══════════════════════════════════════════════════╣");
    println!("║  Status: Running                                  ║");
    println!("║  Port: {:<44}║", port);
    println!("║  GitHub Owner: {:<35}║", owner);
    println!("║  GitHub Repo: {:<36}║", repo);
    println!("╠═══════════════════════════════════════════════════╣");
    println!("║  Endpoints:                                       ║");
    println!("║    GET  /health                                   ║");
    println!("║    GET  /api/github/*                             ║");
    println!("╠═══════════════════════════════════════════════════╣");
    println!("║  ⚡ Powered by Avila/Arxis Libraries              ║");
    println!("║  🔥 Zero tokio/axum dependencies                  ║");
    println!("╚═══════════════════════════════════════════════════╝\n");
}
