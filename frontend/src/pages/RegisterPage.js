import React, { useState } from "react";
import { useFormik } from "formik";
import * as Yup from "yup";
import { useAuth } from "../context/AuthContext";
import { useNavigate, Link } from "react-router-dom";

import {
    TextField,
    Button,
    Container,
    Typography,
    Box,
    Alert,
    Paper,
    CircularProgress
} from "@mui/material";

const validationSchema = Yup.object({
    username: Yup.string()
        .min(3, "Имя пользователя должно содержать минимум 3 символа")
        .max(100, "Имя пользователя не может превышать 100 символов")
        .required("Имя пользователя обязательно"),

    email: Yup.string()
        .email("Некорректный формат email")
        .required("Email обязателен"),

    password: Yup.string()
        .min(6, "Пароль должен содержать минимум 6 символов")
        .required("Пароль обязателен"),

    confirmPassword: Yup.string()
        .oneOf(
            [Yup.ref("password"), null],
            "Пароли не совпадают"
        )
        .required("Подтверждение пароля обязательно")
});

const RegisterPage = () => {
    const { register } = useAuth();
    const navigate = useNavigate();
    const [error, setError] = useState("");

    const formik = useFormik({
        initialValues: {
            username: "",
            email: "",
            password: "",
            confirmPassword: ""
        },

        validationSchema,

        onSubmit: async (values) => {
            setError("");

            const result = await register(
                values.username,
                values.email,
                values.password,
                values.confirmPassword
            );

            if (result?.success) {
                navigate("/");
            } else {
                setError(result.error);
            }
        }
    });

    return (
        <Container maxWidth="sm">
            <Paper elevation={3} sx={{ p: 4, mt: 8 }}>
                <Typography
                    variant="h4"
                    component="h1"
                    gutterBottom
                    align="center"
                >
                    Регистрация
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <form onSubmit={formik.handleSubmit}>
                    <TextField
                        fullWidth
                        id="username"
                        name="username"
                        label="Имя пользователя"
                        value={formik.values.username}
                        onChange={formik.handleChange}
                        error={
                            formik.touched.username &&
                            Boolean(formik.errors.username)
                        }
                        helperText={
                            formik.touched.username &&
                            formik.errors.username
                        }
                        margin="normal"
                    />

                    <TextField
                        fullWidth
                        id="email"
                        name="email"
                        label="Email"
                        type="email"
                        value={formik.values.email}
                        onChange={formik.handleChange}
                        error={
                            formik.touched.email &&
                            Boolean(formik.errors.email)
                        }
                        helperText={
                            formik.touched.email &&
                            formik.errors.email
                        }
                        margin="normal"
                    />

                    <TextField
                        fullWidth
                        id="password"
                        name="password"
                        label="Пароль"
                        type="password"
                        value={formik.values.password}
                        onChange={formik.handleChange}
                        error={
                            formik.touched.password &&
                            Boolean(formik.errors.password)
                        }
                        helperText={
                            formik.touched.password &&
                            formik.errors.password
                        }
                        margin="normal"
                    />

                    <TextField
                        fullWidth
                        id="confirmPassword"
                        name="confirmPassword"
                        label="Подтверждение пароля"
                        type="password"
                        value={formik.values.confirmPassword}
                        onChange={formik.handleChange}
                        error={
                            formik.touched.confirmPassword &&
                            Boolean(formik.errors.confirmPassword)
                        }
                        helperText={
                            formik.touched.confirmPassword &&
                            formik.errors.confirmPassword
                        }
                        margin="normal"
                    />

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        color="primary"
                        disabled={formik.isSubmitting}
                        sx={{ mt: 3, mb: 2 }}
                    >
                        {formik.isSubmitting ? (
                            <CircularProgress size={24} />
                        ) : (
                            "Зарегистрироваться"
                        )}
                    </Button>

                    <Box textAlign="center">
                        <Link to="/login">
                            <Typography variant="body2">
                                Уже есть аккаунт? Войти
                            </Typography>
                        </Link>
                    </Box>
                </form>
            </Paper>
        </Container>
    );
};

export default RegisterPage;