import { Card, CardContent, Grid, Typography, useTheme } from "@mui/material";
import theme from "../../themes";
import { PositionForm } from "./PositionForm";

export const PositionFormWrapper = () => {
  const theme = useTheme();

  return (
    <Grid container>
      <Grid item xs={12}>
        <Card
          sx={{
            border: "1px solid",
            borderColor: theme.palette.primary.light + 75,
            ":hover": {
              boxShadow: "0 2px 14px 0 rgb(32 40 45 / 8%)",
            },
            // maxWidth: { xs: 400, lg: 475 },
            margin: { md: 3 },
            "& > *": {
              flexGrow: 1,
              flexBasis: "50%",
            },
          }}
        >
          <CardContent>
            <PositionForm />
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  );
};
