import React, {
    createContext,
    useState,
    useContext,
    useEffect
} from "react";

import authService from "../services/authService";

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const currentUser = authService.getCurrentUser();
        const isAuthenticated = authService.isAuthenticated();

        if (currentUser && isAuthenticated) {
            setUser(currentUser);
        }

        setLoading(false);
    }, []);

    const register = async (
        username,
        email,
        password,
        confirmPassword
    ) => {
        try {
            await authService.register(
                username,
                email,
                password,
                confirmPassword
            );

            return {
                success: true
            };
        } catch (error) {
            return {
                success: false,
                error: "Ошибка регистрации"
            };
        }
    };

    const login = async (
        username,
        password,
        rememberMe = false
    ) => {
        try {
            const response =
                await authService.login(
                    username,
                    password
                );

            if (response.requires2FA) {
                sessionStorage.setItem(
                    "pending_username",
                    username
                );

                sessionStorage.setItem(
                    "pending_remember",
                    rememberMe
                );

                return {
                    success: false,
                    requires2FA: true
                };
            }

            return {
                success: false,
                error: "Ошибка входа"
            };
        } catch (error) {
            if (error.response?.status === 401) {
                return {
                    success: false,
                    error:
                        "Неверный логин или пароль"
                };
            }

            return {
                success: false,
                error: "Ошибка входа"
            };
        }
    };

    const verify2FA = async (code) => {
        const username =
            sessionStorage.getItem(
                "pending_username"
            );

        const rememberMe =
            sessionStorage.getItem(
                "pending_remember"
            ) === "true";

        const response =
            await authService.verify2FA(
                username,
                code,
                rememberMe
            );

        const currentUser =
            authService.getCurrentUser();

        setUser(currentUser);

        sessionStorage.removeItem(
            "pending_username"
        );

        sessionStorage.removeItem(
            "pending_remember"
        );

        return response;
    };

    const logout = async () => {
        await authService.logout();
        setUser(null);
    };

    const value = {
        user,
        loading,
        register,
        login,
        verify2FA,
        logout,
        isAuthenticated:
            !!user && authService.isAuthenticated()
    };

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
};