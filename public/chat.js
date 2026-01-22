/**
 * Avila Chat Widget - Chat ao Vivo
 * Alternativa ao Tawk.to integrada com Analytics
 */

(function() {
    'use strict';

    const config = {
        apiUrl: document.currentScript.getAttribute('data-api-url') || 'http://localhost:5056',
        siteId: document.currentScript.getAttribute('data-site-id'),
        hubUrl: document.currentScript.getAttribute('data-hub-url') || 'http://localhost:5056/hubs/chat'
    };

    if (!config.siteId) {
        console.error('Avila Chat: site-id is required');
        return;
    }

    // Get visitor ID from analytics
    function getVisitorId() {
        let visitorId = localStorage.getItem('avila_visitor_id');
        if (!visitorId) {
            visitorId = 'v_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            localStorage.setItem('avila_visitor_id', visitorId);
        }
        return visitorId;
    }

    // Load widget config
    fetch(`${config.apiUrl}/api/chat/widget/${config.siteId}`)
        .then(res => res.json())
        .then(data => {
            if (data.success && data.data.enabled) {
                initWidget(data.data.config);
            }
        })
        .catch(err => console.error('Avila Chat error:', err));

    function initWidget(widgetConfig) {
        const position = widgetConfig.position || 'bottom-right';
        const color = widgetConfig.color || '#1976d2';
        const welcomeMessage = widgetConfig.welcomeMessage || 'Olá! Como posso ajudar?';

        // Create widget HTML
        const widgetHTML = `
            <div id="avila-chat-widget" style="position: fixed; ${position.includes('right') ? 'right: 20px' : 'left: 20px'}; ${position.includes('bottom') ? 'bottom: 20px' : 'top: 20px'}; z-index: 9999; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;">
                <div id="avila-chat-bubble" style="width: 60px; height: 60px; background: ${color}; border-radius: 50%; cursor: pointer; box-shadow: 0 4px 12px rgba(0,0,0,0.15); display: flex; align-items: center; justify-content: center; color: white; font-size: 28px;">
                    💬
                </div>
                <div id="avila-chat-window" style="display: none; width: 350px; height: 500px; background: white; border-radius: 12px; box-shadow: 0 8px 24px rgba(0,0,0,0.2); flex-direction: column; overflow: hidden;">
                    <div style="background: ${color}; color: white; padding: 16px; font-weight: 600; display: flex; justify-content: space-between; align-items: center;">
                        <span>Chat ao Vivo</span>
                        <span id="avila-chat-close" style="cursor: pointer; font-size: 20px;">×</span>
                    </div>
                    <div id="avila-chat-messages" style="flex: 1; padding: 16px; overflow-y: auto; background: #f5f5f5;">
                        <div style="background: white; padding: 12px; border-radius: 8px; margin-bottom: 8px;">
                            ${welcomeMessage}
                        </div>
                    </div>
                    <div style="padding: 12px; border-top: 1px solid #e0e0e0; display: flex; gap: 8px;">
                        <input id="avila-chat-input" type="text" placeholder="Digite sua mensagem..." style="flex: 1; padding: 10px; border: 1px solid #ddd; border-radius: 20px; outline: none; font-size: 14px;" />
                        <button id="avila-chat-send" style="background: ${color}; color: white; border: none; border-radius: 50%; width: 40px; height: 40px; cursor: pointer; font-size: 18px;">➤</button>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', widgetHTML);

        const bubble = document.getElementById('avila-chat-bubble');
        const window = document.getElementById('avila-chat-window');
        const closeBtn = document.getElementById('avila-chat-close');
        const input = document.getElementById('avila-chat-input');
        const sendBtn = document.getElementById('avila-chat-send');
        const messages = document.getElementById('avila-chat-messages');

        // Toggle window
        bubble.addEventListener('click', () => {
            if (window.style.display === 'none') {
                window.style.display = 'flex';
                bubble.style.display = 'none';
            }
        });

        closeBtn.addEventListener('click', () => {
            window.style.display = 'none';
            bubble.style.display = 'flex';
        });

        // Send message
        function sendMessage() {
            const message = input.value.trim();
            if (!message) return;

            addMessage(message, 'visitor');
            input.value = '';

            // Send to server (implement SignalR connection here)
            fetch(`${config.apiUrl}/api/chat/send`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    siteId: config.siteId,
                    visitorId: getVisitorId(),
                    message: message
                })
            }).catch(console.error);
        }

        sendBtn.addEventListener('click', sendMessage);
        input.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') sendMessage();
        });

        function addMessage(text, sender) {
            const messageDiv = document.createElement('div');
            messageDiv.style.cssText = `
                background: ${sender === 'visitor' ? color : 'white'};
                color: ${sender === 'visitor' ? 'white' : 'black'};
                padding: 10px 12px;
                border-radius: 8px;
                margin-bottom: 8px;
                max-width: 80%;
                ${sender === 'visitor' ? 'margin-left: auto; text-align: right;' : ''}
            `;
            messageDiv.textContent = text;
            messages.appendChild(messageDiv);
            messages.scrollTop = messages.scrollHeight;
        }

        // TODO: Implement SignalR connection for real-time messages
        console.log('✅ Avila Chat Widget loaded');
    }
})();
