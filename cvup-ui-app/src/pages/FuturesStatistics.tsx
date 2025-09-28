import { observer } from "mobx-react";
import { Grid } from "@mui/material";
import styles from "./FuturesStatistics.module.scss";
import { EditOHLC } from "../components/futuresStatistics/EditOHLC";

export const FuturesStatistics = observer(() => {
  return (
    <Grid container className={styles.pageContainer}>
      <Grid item xs={12} md={12}>
        <EditOHLC />
      </Grid>
    </Grid>
  );
});
