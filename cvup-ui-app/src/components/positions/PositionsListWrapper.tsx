import { Box, Button, Paper } from "@mui/material";
import { PositionsList } from "./PositionsList";
import { GoPlus } from "react-icons/go";
import { useNavigate } from "react-router-dom";

export const PositionsListWrapper = () => {
  const navigate = useNavigate();

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
      <Button onClick={() => navigate("/position")} startIcon={<GoPlus />}>
        Add Position
      </Button>
      <Paper elevation={3}>
        <PositionsList />
      </Paper>
    </Box>
  );
};
