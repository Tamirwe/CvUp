import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../Hooks/useStore";
import {
  TableContainer,
  Paper,
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  FormControl,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Button,
  Box,
} from "@mui/material";
import { format } from "date-fns";
import { useNavigate } from "react-router-dom";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ICandsReport } from "../models/GeneralModels";
import { isMobile } from "react-device-detect";

export const Dashboard = observer(() => {
  const navigate = useNavigate();

  const { candsStore, positionsStore, customersContactsStore } = useStore();

  const [stageType, setStageType] = useState<string>("accepted");
  const [totalRows, setTotalRows] = useState<string>("");

  useEffect(() => {
    positionsStore.getPosTypesCounts();
  }, []);

  useEffect(() => {
    setTotalRows("");

    const total = candsStore.candsReportData?.length;

    if (candsStore.candsReportData && total) {
      if (total === 500) {
        setTotalRows("More then 500 candidate");
      } else {
        setTotalRows(`Total: ${total} candidate`);
      }
    }
  }, [candsStore.candsReportData]);

  const itemColors = [
    "#feeaff",
    "#eaffec",
    "#fff6ea",
    "#eaefff",
    "#ffeaea",
    "#fcffea",
    "#eaf9ff",
    "#f8eaff",
    "#eafffb",
    "#feeaff",
    "#eaffec",
    "#fff6ea",
    "#eaefff",
    "#ffeaea",
    "#fcffea",
    "#eaf9ff",
    "#f8eaff",
    "#eafffb",
  ];

  return (
    <Grid container>
      <Grid item xs={12} lg={12} m={1} mt={5}>
        <h1>Positions - Cv`s count</h1>
      </Grid>
      <Grid item xs={12} lg={12} m={1}>
        <Paper sx={{ width: "100%", overflow: "hidden" }}>
          <Grid container sx={{ direction: "rtl" }}>
            {positionsStore.posTypescountList.map((item, i) => {
              return (
                <Grid key={i} item xs={isMobile ? 6 : 4} p={1}>
                  <Paper
                    sx={{
                      width: "100%",
                      overflow: "hidden",
                    }}
                  >
                    <Box
                      sx={{
                        padding: 2,
                        width: "100%",
                        backgroundColor:
                          itemColors.length > i - 1 ? itemColors[i] : "unset",
                      }}
                    >
                      <div
                        style={{
                          display: "flex",
                          flexDirection: "column",
                          minHeight: "6rem",
                          position: "relative",
                        }}
                      >
                        <div style={{ fontSize: "1.1rem" }}>
                          {item.typeName}
                        </div>

                        <div
                          style={{
                            direction: "ltr",
                            position: "absolute",
                            bottom: 0,
                            left: 0,
                          }}
                        >
                          <div
                            style={{ fontWeight: 700, color: "#616161" }}
                          >{`Today: ${item.todayCount}`}</div>
                          <div
                            style={{ fontWeight: 700, color: "#8f8f8f" }}
                          >{`Yesterday: ${item.yesterdayCount}`}</div>
                        </div>
                      </div>
                    </Box>
                  </Paper>
                </Grid>
              );
            })}
          </Grid>
        </Paper>
      </Grid>
    </Grid>
  );
});
