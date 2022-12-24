import { Box, Button, Paper } from "@mui/material";
import { PositionsList } from "./PositionsList";
import { GoPlus } from "react-icons/go";
import { useNavigate } from "react-router-dom";

export const PositionsListWrapper = () => {
  const navigate = useNavigate();

  return (
    <Box>
      <Box>
        <Button
          sx={{ width: "fit-content" }}
          onClick={() => navigate("/position/0")}
          startIcon={<GoPlus />}
        >
          Add Position
        </Button>
      </Box>
      <Box
        sx={{
          height: "83vh",
          overflowY: "scroll",
          // display: "flex",
          // flexDirection: "column",
          // flexWrap: "wrap",
          // "& > :not(style)": {
          //   m: 1,
          // },
        }}
      >
        {/* <Paper elevation={3}> */}
        <PositionsList />
        {/* </Paper> */}
      </Box>
      <Box>
        <Button
          sx={{ width: "fit-content" }}
          onClick={() => navigate("/position/0")}
          startIcon={<GoPlus />}
        >
          Add Position
        </Button>
      </Box>
    </Box>
  );
};
