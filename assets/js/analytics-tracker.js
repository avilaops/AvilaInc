// analytics-tracker.js - Integração JavaScript para sites

class AvxAnalytics {
    constructor(measurementId, endpoint = 'http://localhost:8080') {
        this.measurementId = measurementId;
        this.endpoint = endpoint;
        this.userId = this.getUserId();
        this.sessionId = this.getSessionId();
    }

    // Gera ou recupera ID do usuário
    getUserId() {
        let userId = localStorage.getItem('avx_user_id');
        if (!userId) {
            userId = 'user_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            localStorage.setItem('avx_user_id', userId);
        }
        return userId;
    }

    // Gera ou recupera ID da sessão
    getSessionId() {
        let sessionId = sessionStorage.getItem('avx_session_id');
        if (!sessionId) {
            sessionId = 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            sessionStorage.setItem('avx_session_id', sessionId);
        }
        return sessionId;
    }

    // Rastreia visualização de página automaticamente
    trackPageView(pageTitle, pageLocation) {
        const event = {
            event_type: 'page_view',
            page_title: pageTitle || document.title,
            page_location: pageLocation || window.location.href,
            page_referrer: document.referrer,
            user_id: this.userId,
            session_id: this.sessionId,
            timestamp: new Date().toISOString()
        };

        this.sendEvent(event);
    }

    // Rastreia cliques em elementos
    trackClick(elementId, elementText, customData = {}) {
        const event = {
            event_type: 'click',
            element_id: elementId,
            element_text: elementText,
            user_id: this.userId,
            session_id: this.sessionId,
            custom_dimensions: customData,
            timestamp: new Date().toISOString()
        };

        this.sendEvent(event);
    }

    // Rastreia eventos customizados
    trackCustom(eventName, customData = {}) {
        const event = {
            event_type: 'custom',
            name: eventName,
            user_id: this.userId,
            session_id: this.sessionId,
            custom_dimensions: customData,
            timestamp: new Date().toISOString()
        };

        this.sendEvent(event);
    }

    // Rastreia formulários
    trackFormSubmit(formId, formData = {}) {
        const event = {
            event_type: 'form_submit',
            form_id: formId,
            user_id: this.userId,
            session_id: this.sessionId,
            custom_dimensions: formData,
            timestamp: new Date().toISOString()
        };

        this.sendEvent(event);
    }

    // Rastreia downloads de arquivo
    trackDownload(fileName, fileUrl) {
        const event = {
            event_type: 'file_download',
            file_name: fileName,
            link_url: fileUrl,
            user_id: this.userId,
            session_id: this.sessionId,
            timestamp: new Date().toISOString()
        };

        this.sendEvent(event);
    }

    // Envia evento para o servidor
    async sendEvent(eventData) {
        try {
            const response = await fetch(`${this.endpoint}/api/v1/collect`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Measurement-Id': this.measurementId
                },
                body: JSON.stringify(eventData)
            });

            if (!response.ok) {
                console.warn('Analytics tracking failed:', response.status);
            }
        } catch (error) {
            console.warn('Analytics tracking error:', error);
            // Em produção, você pode querer enviar para um serviço de logging
        }
    }

    // Inicialização automática
    init() {
        // Rastreia page view inicial
        this.trackPageView();

        // Rastreia cliques automaticamente (opcional)
        document.addEventListener('click', (e) => {
            const target = e.target.closest('[data-track-click]');
            if (target) {
                const eventName = target.getAttribute('data-track-click');
                const eventData = target.getAttribute('data-track-data');
                this.trackCustom(eventName, eventData ? JSON.parse(eventData) : {});
            }
        });

        // Rastreia formulários automaticamente
        document.addEventListener('submit', (e) => {
            const form = e.target;
            if (form.id) {
                const formData = {};
                new FormData(form).forEach((value, key) => {
                    formData[key] = value;
                });
                this.trackFormSubmit(form.id, formData);
            }
        });
    }
}

// Inicialização global
window.AvxAnalytics = AvxAnalytics;
