// Service Worker para PWA
const CACHE_NAME = 'avila-dashboard-v4';
const urlsToCache = [
  '/',
  '/dashboard.html',
  '/css/dashboard.css',
  '/css/style.css',
  '/css/components.css',
  '/js/dashboard.js',
  '/js/auth.js'
];

// Instalar Service Worker
self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => {
        console.log('📦 Cache aberto');
        return cache.addAll(urlsToCache);
      })
  );
});

// Ativar Service Worker
self.addEventListener('activate', event => {
  event.waitUntil(
    caches.keys().then(cacheNames => {
      return Promise.all(
        cacheNames.map(cacheName => {
          if (cacheName !== CACHE_NAME) {
            console.log('🗑️ Removendo cache antigo:', cacheName);
            return caches.delete(cacheName);
          }
        })
      );
    })
  );
});

// Interceptar requisições
self.addEventListener('fetch', event => {
  event.respondWith(
    caches.match(event.request)
      .then(response => {
        // Cache hit - retornar resposta do cache
        if (response) {
          return response;
        }

        // Clonar requisição
        const fetchRequest = event.request.clone();

        return fetch(fetchRequest).then(response => {
          // Verificar se resposta é válida
          if (!response || response.status !== 200 || response.type !== 'basic') {
            return response;
          }

          // Clonar resposta
          const responseToCache = response.clone();

          caches.open(CACHE_NAME)
            .then(cache => {
              // Verificar se a URL não é de extensão do Chrome
              if (!event.request.url.startsWith('chrome-extension://') && 
                  !event.request.url.startsWith('moz-extension://')) {
                cache.put(event.request, responseToCache);
              }
            });

          return response;
        });
      })
  );
});
