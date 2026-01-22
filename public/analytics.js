/**
 * Avila Analytics - Sistema de Analytics Próprio
 * Substitui: Google Analytics, Facebook Pixel, Hotjar
 * 
 * Uso:
 * <script src="https://sua-api.com/analytics.js" data-site-id="SEU_SITE_ID"></script>
 */

(function() {
    'use strict';

    const config = {
        apiUrl: document.currentScript.getAttribute('data-api-url') || 'http://localhost:5056/api/analytics',
        siteId: document.currentScript.getAttribute('data-site-id'),
        autoTrack: document.currentScript.getAttribute('data-auto-track') !== 'false'
    };

    if (!config.siteId) {
        console.error('Avila Analytics: site-id is required');
        return;
    }

    // Generate unique visitor ID
    function getVisitorId() {
        let visitorId = localStorage.getItem('avila_visitor_id');
        if (!visitorId) {
            visitorId = 'v_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            localStorage.setItem('avila_visitor_id', visitorId);
        }
        return visitorId;
    }

    // Generate session ID (expires after 30 minutes of inactivity)
    function getSessionId() {
        const sessionKey = 'avila_session_id';
        const sessionTimeKey = 'avila_session_time';
        const sessionTimeout = 30 * 60 * 1000; // 30 minutes

        let sessionId = sessionStorage.getItem(sessionKey);
        let sessionTime = sessionStorage.getItem(sessionTimeKey);

        if (!sessionId || !sessionTime || Date.now() - parseInt(sessionTime) > sessionTimeout) {
            sessionId = 's_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            sessionStorage.setItem(sessionKey, sessionId);
        }
        
        sessionStorage.setItem(sessionTimeKey, Date.now().toString());
        return sessionId;
    }

    // Get user info
    function getUserInfo() {
        const ua = navigator.userAgent;
        return {
            browser: getBrowser(ua),
            os: getOS(ua),
            device: getDevice(),
            screenWidth: window.screen.width,
            screenHeight: window.screen.height
        };
    }

    function getBrowser(ua) {
        if (ua.indexOf('Chrome') > -1) return 'Chrome';
        if (ua.indexOf('Safari') > -1) return 'Safari';
        if (ua.indexOf('Firefox') > -1) return 'Firefox';
        if (ua.indexOf('Edge') > -1) return 'Edge';
        return 'Unknown';
    }

    function getOS(ua) {
        if (ua.indexOf('Win') > -1) return 'Windows';
        if (ua.indexOf('Mac') > -1) return 'macOS';
        if (ua.indexOf('Linux') > -1) return 'Linux';
        if (ua.indexOf('Android') > -1) return 'Android';
        if (ua.indexOf('iOS') > -1) return 'iOS';
        return 'Unknown';
    }

    function getDevice() {
        const width = window.innerWidth;
        if (width < 768) return 'Mobile';
        if (width < 1024) return 'Tablet';
        return 'Desktop';
    }

    // Get UTM parameters
    function getUTMParams() {
        const params = new URLSearchParams(window.location.search);
        return {
            utmSource: params.get('utm_source'),
            utmMedium: params.get('utm_medium'),
            utmCampaign: params.get('utm_campaign')
        };
    }

    // Track pageview
    function trackPageview() {
        const userInfo = getUserInfo();
        const utmParams = getUTMParams();

        fetch(`${config.apiUrl}/track/pageview`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                siteId: config.siteId,
                visitorId: getVisitorId(),
                sessionId: getSessionId(),
                url: window.location.href,
                title: document.title,
                referrer: document.referrer,
                ...userInfo,
                ...utmParams
            })
        }).catch(err => console.error('Avila Analytics error:', err));
    }

    // Track custom event
    window.avilaTrack = function(eventName, eventData = {}) {
        fetch(`${config.apiUrl}/track/event`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                siteId: config.siteId,
                visitorId: getVisitorId(),
                sessionId: getSessionId(),
                eventName: eventName,
                eventCategory: eventData.category,
                eventLabel: eventData.label,
                eventValue: eventData.value,
                url: window.location.href,
                metadata: eventData.metadata
            })
        }).catch(err => console.error('Avila Analytics error:', err));
    };

    // Auto-track clicks on buttons and links
    if (config.autoTrack) {
        document.addEventListener('click', function(e) {
            const target = e.target.closest('a, button');
            if (target) {
                const eventData = {
                    category: target.tagName.toLowerCase(),
                    label: target.textContent.trim() || target.getAttribute('aria-label'),
                    metadata: {
                        href: target.getAttribute('href') || '',
                        id: target.id || '',
                        class: target.className || ''
                    }
                };
                avilaTrack('click', eventData);
            }
        });
    }

    // Track initial pageview
    if (document.readyState === 'complete') {
        trackPageview();
    } else {
        window.addEventListener('load', trackPageview);
    }

    // Track page changes (for SPAs)
    let lastUrl = window.location.href;
    new MutationObserver(() => {
        const currentUrl = window.location.href;
        if (currentUrl !== lastUrl) {
            lastUrl = currentUrl;
            trackPageview();
        }
    }).observe(document.body, { childList: true, subtree: true });

    // === HEATMAP TRACKING ===
    
    // Track clicks with coordinates
    document.addEventListener('click', function(e) {
        fetch(`${config.apiUrl}/heatmaps/track/click`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                siteId: config.siteId,
                visitorId: getVisitorId(),
                sessionId: getSessionId(),
                url: window.location.href,
                x: e.pageX,
                y: e.pageY,
                screenWidth: window.innerWidth,
                screenHeight: window.innerHeight,
                elementTag: e.target.tagName,
                elementId: e.target.id || null,
                elementClass: e.target.className || null
            })
        }).catch(() => {});
    });

    // Track scroll depth
    let maxScrollDepth = 0;
    let scrollTimeout;
    
    window.addEventListener('scroll', function() {
        const scrollPercentage = Math.round((window.scrollY / (document.documentElement.scrollHeight - window.innerHeight)) * 100);
        
        if (scrollPercentage > maxScrollDepth) {
            maxScrollDepth = scrollPercentage;
            
            clearTimeout(scrollTimeout);
            scrollTimeout = setTimeout(() => {
                fetch(`${config.apiUrl}/heatmaps/track/scroll`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        siteId: config.siteId,
                        visitorId: getVisitorId(),
                        sessionId: getSessionId(),
                        url: window.location.href,
                        maxDepth: maxScrollDepth
                    })
                }).catch(() => {});
            }, 1000);
        }
    });

    console.log('✅ Avila Analytics loaded for site:', config.siteId);
})();
