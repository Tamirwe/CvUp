import { observer } from "mobx-react";
import { Grid } from "@mui/material";
import styles from "./FuturesStatistics.module.scss";
import {
  EditOHLC,
  EditOHLCRefType,
} from "../components/futuresStatistics/EditOHLC";
import { useStore } from "../Hooks/useStore";
import { useEffect, useRef } from "react";
import { ListOHLC } from "../components/futuresStatistics/ListOHLC";
import { Iohlc } from "../models/FuStatModel";
import { ChartCandlestick } from "../components/futuresStatistics/ChartCandlestick";

export const FuturesStatistics = observer(() => {
  const editOHLCRef = useRef<EditOHLCRefType>(null);

  const { futuresStatisticStore } = useStore();

  useEffect(() => {
    futuresStatisticStore.getDayOhlcList();
  }, []);

  useEffect(() => {
    futuresStatisticStore.calculateAveragesMedians();
  }, [futuresStatisticStore.dailyStatList]);

  return (
    <Grid container className={styles.pageContainer}>
      <Grid item xs={12} md={5}>
        <EditOHLC ref={editOHLCRef} />
        <ListOHLC
          onEdit={(ohlcRow: Iohlc) => {
            editOHLCRef.current?.editRow(ohlcRow);
          }}
        />
      </Grid>
      <Grid item xs={12} md={6}>
        <ChartCandlestick />
      </Grid>
    </Grid>
  );
});
