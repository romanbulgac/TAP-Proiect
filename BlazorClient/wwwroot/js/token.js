console.log('token.js: script started');

window.tokenStorage = {
    getAccessToken: function() {
        console.log('token.js: tokenStorage.getAccessToken CALLED');
        const token = localStorage.getItem('access_token');
        console.log('token.js: tokenStorage.getAccessToken RETURNING', token);
        return token;
    },
    setAccessToken: function(token, expiresInSeconds) {
        console.log('token.js: tokenStorage.setAccessToken CALLED with token:', token, 'expiresInSeconds:', expiresInSeconds);
        localStorage.setItem('access_token', token);
        if (expiresInSeconds) {
            const expiryTime = Date.now() + (expiresInSeconds * 1000);
            localStorage.setItem('token_expiry', expiryTime.toString());
        }
    },
    removeAccessToken: function() {
        console.log('token.js: tokenStorage.removeAccessToken CALLED');
        localStorage.removeItem('access_token');
        localStorage.removeItem('token_expiry');
    },
    getRefreshToken: function() {
        console.log('token.js: tokenStorage.getRefreshToken CALLED');
        return localStorage.getItem('refresh_token');
    },
    setRefreshToken: function(token) {
        console.log('token.js: tokenStorage.setRefreshToken CALLED with token:', token);
        localStorage.setItem('refresh_token', token);
    },
    removeRefreshToken: function() {
        console.log('token.js: tokenStorage.removeRefreshToken CALLED');
        localStorage.removeItem('refresh_token');
    },
    clearTokens: function() {
        console.log('token.js: tokenStorage.clearTokens CALLED');
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        localStorage.removeItem('token_expiry');
    }
};

console.log('token.js: window.tokenStorage DEFINED:', window.tokenStorage);

// All other functions (initializeTokenRefresh, storeTokens, checkTokenExpiry, refreshToken, fetch override)
// are temporarily removed for this debugging step.
