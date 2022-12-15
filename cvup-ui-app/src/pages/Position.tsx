import { observer } from "mobx-react";
import { Grid } from "@mui/material";
import { PositionFormWrapper } from "../components/positions/PositionFormWrapper";

export const Position = observer(() => {
  return (
    <Grid container>
      <Grid item xs={12} md={12}>
        <PositionFormWrapper />
      </Grid>
    </Grid>
  );
});
