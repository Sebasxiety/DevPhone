/**
 * Sistema de autenticación frontend para DevPhone
 * Maneja JWT tokens, refresh automático y redirecciones
 */

class AuthManager {
    constructor() {
        this.refreshTokenEndpoint = '/Account/RefreshToken';
        this.loginEndpoint = '/Account/Login';
        this.logoutEndpoint = '/Account/Logout';
        this.isRefreshing = false;
        this.failedQueue = [];
        
        this.setupInterceptors();
        this.setupTokenRefresh();
    }

    /**
     * Configura interceptores para peticiones AJAX
     */
    setupInterceptors() {
        // Interceptor para jQuery AJAX
        $(document).ajaxError((event, xhr, settings) => {
            if (xhr.status === 401 && !settings.url.includes(this.loginEndpoint)) {
                this.handleUnauthorized();
            }
        });

        // Interceptor para fetch API
        const originalFetch = window.fetch;
        window.fetch = async (...args) => {
            const response = await originalFetch(...args);
            
            if (response.status === 401 && !args[0].includes(this.loginEndpoint)) {
                this.handleUnauthorized();
            }
            
            return response;
        };
    }

    /**
     * Configura el refresh automático de tokens
     */
    setupTokenRefresh() {
        // Intentar refresh cada 50 minutos (los tokens expiran en 1 hora)
        setInterval(() => {
            this.refreshTokenSilently();
        }, 50 * 60 * 1000);
    }

    /**
     * Maneja respuestas 401 (no autorizado)
     */
    async handleUnauthorized() {
        if (this.isRefreshing) {
            return new Promise((resolve, reject) => {
                this.failedQueue.push({ resolve, reject });
            });
        }

        this.isRefreshing = true;

        try {
            const refreshed = await this.refreshTokenSilently();
            
            if (refreshed) {
                // Procesar cola de peticiones fallidas
                this.failedQueue.forEach(({ resolve }) => resolve());
                this.failedQueue = [];
            } else {
                this.redirectToLogin();
            }
        } catch (error) {
            console.error('Error al refrescar token:', error);
            this.redirectToLogin();
        } finally {
            this.isRefreshing = false;
        }
    }

    /**
     * Refresca el token silenciosamente
     */
    async refreshTokenSilently() {
        try {
            const response = await $.ajax({
                url: this.refreshTokenEndpoint,
                method: 'POST',
                headers: {
                    'X-CSRF-TOKEN': window.csrfToken
                }
            });

            if (response.success) {
                console.log('Token refrescado exitosamente');
                return true;
            } else {
                console.warn('No se pudo refrescar el token:', response.message);
                return false;
            }
        } catch (error) {
            console.error('Error al refrescar token:', error);
            return false;
        }
    }

    /**
     * Redirige al login
     */
    redirectToLogin() {
        const currentUrl = encodeURIComponent(window.location.pathname + window.location.search);
        window.location.href = `/Account/Login?returnUrl=${currentUrl}`;
    }

    /**
     * Realiza login con AJAX
     */
    async login(username, password) {
        try {
            const response = await $.ajax({
                url: this.loginEndpoint,
                method: 'POST',
                data: {
                    Username: username,
                    Password: password,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response.success) {
                // Guardar tiempo de login
                localStorage.setItem('loginTime', new Date().toISOString());
                
                // Mostrar notificación de éxito
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        title: '¡Bienvenido!',
                        text: 'Has iniciado sesión correctamente',
                        icon: 'success',
                        timer: 2000,
                        showConfirmButton: false
                    });
                }
                
                // Redirigir después de un breve delay
                setTimeout(() => {
                    const returnUrl = new URLSearchParams(window.location.search).get('returnUrl');
                    window.location.href = returnUrl || response.redirectUrl || '/Home/Dashboard';
                }, 1500);
                
                return { success: true, message: response.message };
            } else {
                return { success: false, message: response.message, errors: response.errors };
            }
        } catch (error) {
            console.error('Error en login:', error);
            return { 
                success: false, 
                message: 'Error de conexión. Inténtalo de nuevo.',
                errors: ['Error de red']
            };
        }
    }

    /**
     * Realiza logout
     */
    async logout() {
        try {
            await $.ajax({
                url: this.logoutEndpoint,
                method: 'POST',
                headers: {
                    'X-CSRF-TOKEN': window.csrfToken,
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            // Limpiar datos de sesión
            localStorage.removeItem('loginTime');
            
            // Mostrar notificación
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    title: 'Sesión cerrada',
                    text: 'Has cerrado sesión correctamente',
                    icon: 'info',
                    timer: 2000,
                    showConfirmButton: false
                });
            }
            
            // Redirigir al login
            setTimeout(() => {
                window.location.href = '/Account/Login';
            }, 1500);
        } catch (error) {
            console.error('Error en logout:', error);
            
            // Limpiar datos de sesión incluso si hay error
            localStorage.removeItem('loginTime');
            
            // Aún así redirigir al login
            window.location.href = '/Account/Login';
        }
    }

    /**
     * Verifica si el usuario está autenticado
     */
    isAuthenticated() {
        // En este caso, verificamos si hay cookies de autenticación
        // o si el usuario está en una página que requiere autenticación
        return document.body.classList.contains('authenticated') || 
               $('meta[name="user-authenticated"]').attr('content') === 'true';
    }

    /**
     * Muestra notificaciones de autenticación
     */
    showNotification(message, type = 'info') {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: type === 'error' ? 'Error' : 'Información',
                text: message,
                icon: type,
                timer: 3000,
                showConfirmButton: false
            });
        } else {
            alert(message);
        }
    }
}

// Inicializar el gestor de autenticación cuando el DOM esté listo
$(document).ready(function() {
    window.authManager = new AuthManager();
    
    // Configurar el formulario de login si existe
    $('#loginForm').on('submit', async function(e) {
        e.preventDefault();
        
        const username = $('#Username').val();
        const password = $('#Password').val();
        
        if (!username || !password) {
            window.authManager.showNotification('Por favor, completa todos los campos', 'warning');
            return;
        }

        const result = await window.authManager.login(username, password);
        
        if (!result.success) {
            window.authManager.showNotification(result.message, 'error');
        }
    });

    // Configurar botones de logout
    $('form[action*="Logout"]').on('submit', async function(e) {
        e.preventDefault();
        await window.authManager.logout();
    });
});

// Exportar para uso global
window.AuthManager = AuthManager;