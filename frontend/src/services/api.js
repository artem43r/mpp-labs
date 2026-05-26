import axios from "axios";
import authService from "./authService";

const api = axios.create({
    baseURL: "http://localhost:5025/api",
    headers: {
        "Content-Type": "application/json"
    },
    withCredentials: true
});

api.interceptors.request.use(
    (config) => {
        const token =
            localStorage.getItem("access_token") ||
            sessionStorage.getItem("access_token");

        if (token) {
            config.headers.Authorization =
                `Bearer ${token}`;
        }

        return config;
    },
    (error) => Promise.reject(error)
);

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (
            error.response?.status === 401 &&
            !originalRequest._retry
        ) {
            originalRequest._retry = true;

            const newTokens =
                await authService.refreshToken();

            if (newTokens) {
                originalRequest.headers.Authorization =
                    `Bearer ${newTokens.accessToken}`;

                return api(originalRequest);
            }
        }

        await authService.logout();

        window.location.href = "/login";

        return Promise.reject(error);
    }
);

export default api;