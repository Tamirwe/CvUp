import { Card, CardContent, useTheme } from "@mui/material";
import { RegisterWrapper } from "./RegisterWrapper";

interface props {
  children: React.ReactNode;
}

export const AuthCardWrapper = ({ children }: props) => {
  const theme = useTheme();

  return (
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
      <CardContent>{children}</CardContent>
    </Card>
  );
};
