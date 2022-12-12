import { Box, Button, Link, Paper, Stack, Typography } from "@mui/material";
import { PositionList } from "./PositionList";
import { GoPlus } from "react-icons/go";

export const PositionListWrapper = () => {
  return (
    <Box
      sx={{
        display: "flex",
        flexWrap: "wrap",
        "& > :not(style)": {
          m: 1,
        },
      }}
    >
      <Link href="/position" underline="hover">
        <Stack direction="row" alignItems="center" gap={1}>
          <GoPlus color="primary" />
          New Position
        </Stack>
      </Link>

      <Paper elevation={3}>
        <PositionList />
      </Paper>
    </Box>
  );
};
