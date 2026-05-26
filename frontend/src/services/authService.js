import axios from "axios";

const API_URL = "http://localhost:5025/api";

const authService = {
    async register(username, email, password, confirmPassword) {
        const response = await axios.post(
            `${API_URL}/auth/register`,
            {
                Username: username,
                Email: email,
                Password: password,
                ConfirmPassword: confirmPassword
            },
            {
                withCredentials: true
            }
        );

        return response.data;
    },

    async login(username, password) {
        const response = await axios.post(
            `${API_URL}/auth/login`,
            {
                username,
                password
            },
            {
                withCredentials: true
            }
        );

        return response.data;
    },

    async verify2FA(
        username,
        code,
        rememberMe = false
    ) {
        const response = await axios.post(
            `${API_URL}/auth/verify-2fa`,
            {
                username,
                code
            },
            {
                withCredentials: true
            }
        );

        if (response.data.accessToken) {
            const storage = rememberMe
                ? localStorage
                : sessionStorage;

            [localStorage, sessionStorage].forEach(
                (s) => {
                    s.removeItem("access_token");
                    s.removeItem("user");
                }
            );

            storage.setItem(
                "access_token",
                response.data.accessToken
            );

            storage.setItem(
                "user",
                JSON.stringify({ username })
            );
        }

        return response.data;
    },

    async refreshToken() {
        try {
            const response = await axios.post(
                `${API_URL}/auth/refresh`,
                {},
                {
                    withCredentials: true
                }
            );

            const storage =
                localStorage.getItem("access_token")
                    ? localStorage
                    : sessionStorage;

            storage.setItem(
                "access_token",
                response.data.accessToken
            );

            return response.data;
        } catch {
            this.logout();
            return null;
        }
    },

    async logout() {
        try {
            await axios.post(
                `${API_URL}/auth/logout`,
                {},
                {
                    withCredentials: true
                }
            );
        } catch { }

        [localStorage, sessionStorage].forEach(
            (storage) => {
                storage.removeItem("access_token");
                storage.removeItem("user");
            }
        );
    },

    getCurrentUser() {
        const user =
            localStorage.getItem("user") ||
            sessionStorage.getItem("user");

        return user ? JSON.parse(user) : null;
    },

    isAuthenticated() {
        return !!(
            localStorage.getItem("access_token") ||
            sessionStorage.getItem("access_token")
        );
    }
};

export default authService;