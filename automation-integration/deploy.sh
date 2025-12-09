#!/bin/bash
# Deploy script for Avila Automation Integration

set -e

echo "🚀 Avila Automation Integration - Deploy Script"
echo "================================================"

# Build backend Rust
echo "📦 Building Rust backend..."
cd backend-rs
cargo build --release
cd ..

echo "✅ Build completed!"
echo ""
echo "📊 Binary size:"
ls -lh backend-rs/target/release/automation-integration* 2>/dev/null || ls -lh backend-rs/target/release/automation-integration.exe

echo ""
echo "🎯 To run the server:"
echo "  PORT=3005 ./backend-rs/target/release/automation-integration"
echo ""
echo "Or with environment variables:"
echo "  PORT=3005 GITHUB_TOKEN=your_token ./backend-rs/target/release/automation-integration"
