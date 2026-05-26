import React, { useState } from "react";
import { useFormik } from "formik";
import * as Yup from "yup";
import { useAuth } from "../context/AuthContext";
import { useNavigate } from "react-router-dom";

import {
    TextField,
    Button,
    Container,
    Typography,
    Alert,
    Paper,
    CircularProgress
} from "@mui/material";

const validationSchema = Yup.object({
    code: Yup.string()
        .length(6, "Код должен содержать 6 цифр")
        .required("Введите код")
});

const Verify2FAPage = () => {
    const { verify2FA } = useAuth();
    const navigate = useNavigate();
    const [error, setError] = useState("");

    const formik = useFormik({
        initialValues: {
            code: ""
        },

        validationSchema,

        onSubmit: async (values) => {
            setError("");

            try {
                await verify2FA(values.code);
                navigate("/");
            } catch {
                setError("Неверный код");
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
                    Подтверждение входа
                </Typography>

                <Typography
                    variant="body1"
                    align="center"
                    sx={{ mb: 2 }}
                >
                    Введите 6-значный код
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}

                <form onSubmit={formik.handleSubmit}>
                    <TextField
                        fullWidth
                        id="code"
                        name="code"
                        label="Код подтверждения"
                        value={formik.values.code}
                        onChange={formik.handleChange}
                        error={
                            formik.touched.code &&
                            Boolean(formik.errors.code)
                        }
                        helperText={
                            formik.touched.code &&
                            formik.errors.code
                        }
                        margin="normal"
                    />

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        disabled={formik.isSubmitting}
                        sx={{ mt: 3 }}
                    >
                        {formik.isSubmitting ? (
                            <CircularProgress size={24} />
                        ) : (
                            "Подтвердить"
                        )}
                    </Button>
                </form>
            </Paper>
        </Container>
    );
};

export default Verify2FAPage;