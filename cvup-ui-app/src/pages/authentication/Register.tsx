import {
  Box,
  Card,
  CardContent,
  Button,
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  Grid,
  IconButton,
  InputAdornment,
  InputLabel,
  OutlinedInput,
  Stack,
  TextField,
  Typography,
  useMediaQuery,
  useTheme,
  Divider,
} from "@mui/material";
import { values } from "mobx";
import { useEffect, useState } from "react";
import theme from "../../themes";

import { MdOutlineVisibility, MdOutlineVisibilityOff } from "react-icons/md";
import { TextFieldForm } from "../../components/controls/TextFieldForm";
import { Link } from "react-router-dom";

interface props {}

export const Register = ({}: props) => {
  const theme = useTheme();
  const matchDownSM = useMediaQuery(theme.breakpoints.down("md"));
  const [isShowPassword, setIsShowPassword] = useState(false);

  const handlePasswordChange = () => {};
  const handleUserNameChange = () => {};
  const handleCompanyChange = () => {};

  return (
    <Grid container direction="column" sx={{ minHeight: "100vh" }}>
      <Grid item xs={12}>
        <Grid
          container
          justifyContent="center"
          alignItems="center"
          sx={{ minHeight: "calc(100vh - 68px)" }}
        >
          <Card
            sx={{
              border: "1px solid",
              borderColor: theme.palette.primary.light + 75,
              ":hover": {
                boxShadow: "0 2px 14px 0 rgb(32 40 45 / 8%)",
              },
              maxWidth: { xs: 400, lg: 475 },
              margin: { xs: 2.5, md: 3 },
              "& > *": {
                flexGrow: 1,
                flexBasis: "50%",
              },
            }}
          >
            <CardContent>
              <Grid item xs={12}>
                <Grid
                  container
                  direction={matchDownSM ? "column-reverse" : "row"}
                  alignItems="center"
                  justifyContent="center"
                >
                  <Grid item>
                    <Stack
                      alignItems="center"
                      justifyContent="center"
                      spacing={1}
                    >
                      <Typography
                        color={theme.palette.secondary.main}
                        gutterBottom
                        variant={matchDownSM ? "h3" : "h2"}
                      >
                        Sign up
                      </Typography>
                      <Typography
                        variant="caption"
                        fontSize="16px"
                        textAlign={matchDownSM ? "center" : "inherit"}
                      >
                        Enter your credentials to continue
                      </Typography>
                    </Stack>
                  </Grid>
                </Grid>
              </Grid>
              <Grid item xs={12}>
                <form noValidate>
                  <Grid container spacing={matchDownSM ? 0 : 2}>
                    <Grid item xs={12} sm={6}>
                      <TextField
                        fullWidth
                        required
                        margin="normal"
                        type="text"
                        id="lastNameTxt"
                        label="First Name"
                        variant="outlined"
                        onChange={handleCompanyChange}
                      />
                    </Grid>
                    <Grid item xs={12} sm={6}>
                      <TextField
                        fullWidth
                        required
                        margin="normal"
                        type="text"
                        id="firstNameTxt"
                        label="Last Name"
                        variant="outlined"
                        onChange={handleCompanyChange}
                      />
                    </Grid>
                  </Grid>
                  <TextField
                    fullWidth
                    required
                    margin="normal"
                    type="text"
                    id="companyNameTxt"
                    label="Company"
                    variant="outlined"
                    onChange={handleCompanyChange}
                  />
                  <TextField
                    fullWidth
                    required
                    margin="normal"
                    type="text"
                    id="userNameTxt"
                    label="Email Address / Username"
                    variant="outlined"
                    onChange={handleUserNameChange}
                  />
                  <TextField
                    fullWidth
                    required
                    margin="normal"
                    type={isShowPassword ? "text" : "password"}
                    id="passwordTxt"
                    label="Password"
                    variant="outlined"
                    onChange={handlePasswordChange}
                    InputProps={{
                      endAdornment: (
                        <InputAdornment position="end">
                          <IconButton
                            aria-label="toggle password visibility"
                            onClick={() => setIsShowPassword(!isShowPassword)}
                            edge="end"
                          >
                            {isShowPassword ? (
                              <MdOutlineVisibilityOff />
                            ) : (
                              <MdOutlineVisibility />
                            )}
                          </IconButton>
                        </InputAdornment>
                      ),
                    }}
                  />
                  <FormControlLabel
                    control={<Checkbox />}
                    label={
                      <Typography variant="subtitle1">
                        Agree with &nbsp;
                        <Typography variant="subtitle1" component={Link} to="#">
                          Terms & Condition.
                        </Typography>
                      </Typography>
                    }
                  />
                  <Box sx={{ mt: 2 }}>
                    <Button
                      // disableElevation
                      // disabled={isSubmitting}
                      fullWidth
                      size="large"
                      type="submit"
                      variant="contained"
                      color="secondary"
                    >
                      Sign up
                    </Button>
                  </Box>
                </form>
              </Grid>
              <Grid>
                <Divider sx={{ mt: 2 }} />
              </Grid>
              <Grid item xs={12} sx={{ mt: 2 }}>
                <Grid
                  item
                  container
                  direction="column"
                  alignItems="center"
                  xs={12}
                >
                  <Typography
                    component={Link}
                    to="/pages/login/login3"
                    variant="subtitle1"
                    sx={{ textDecoration: "none" }}
                  >
                    Already have an account?
                  </Typography>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Grid>
  );
};
