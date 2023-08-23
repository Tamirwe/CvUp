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
} from "@mui/material";
import { format } from "date-fns";
import { useNavigate } from "react-router-dom";
import { CandsSourceEnum } from "../models/GeneralEnums";
import { ICandsReport } from "../models/GeneralModels";

export const CandidatesReport = observer(() => {
  const navigate = useNavigate();

  const { candsStore, positionsStore, customersContactsStore } = useStore();

  const [stageType, setStageType] = useState<string>("accepted");
  const [totalRows, setTotalRows] = useState<string>("");

  useEffect(() => {
    candsStore.getCandsReport(stageType);
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

  const handleCandClick = async (row: ICandsReport) => {
    await positionsStore.positionClick(row.positionId!, true);

    const cand = candsStore.posCandsList.find(
      (x) => x.candidateId === row.candidateId
    );

    if (cand) {
      if (window.location.pathname !== "/cv") {
        navigate(`/cv`);
      }

      candsStore.displayCv(cand, CandsSourceEnum.Position);
      positionsStore.setRelatedPositionToCandDisplay();
      candsStore.setDisplayCandOntopPCList();
    }
  };

  return (
    <Grid container>
      <Grid item xs={12} lg={12} m={1} mt={5}>
        <h1>Candidates Report</h1>
      </Grid>
      <Grid item xs={12} lg={6} m={1} mt={3}>
        <FormControl
          fullWidth
          // variant="standard"
          // sx={{ minWidth: 250 }}
        >
          <InputLabel id="stageSelectlabel">
            Candidate status after send
          </InputLabel>
          <Select
            sx={{
              direction: "ltr",
              "& .MuiSelect-select": {
                color: candsStore.posStages?.find(
                  (x) => x.stageType === stageType
                )?.color,
                fontWeight: "bold",
              },
            }}
            id="stageSelect"
            label="Candidate status after send"
            value={stageType || ""}
            onChange={async (e) => {
              setStageType(e.target.value);
              await candsStore.getCandsReport(e.target.value);
            }}
          >
            <MenuItem value="" key="0"></MenuItem>
            {candsStore.posStages?.map((item, ind) => {
              // console.log(key, index);
              return (
                <MenuItem
                  sx={{ color: item.color }}
                  key={ind}
                  value={item.stageType}
                >
                  {item.name}
                </MenuItem>
              );
            })}
          </Select>
        </FormControl>
      </Grid>
      <Grid item xs={12} lg={12} m={1}>
        <Paper sx={{ width: "100%", overflow: "hidden" }}>
          <TableContainer sx={{ maxHeight: "60vh" }}>
            <Table
              stickyHeader
              aria-label="sticky table"
              sx={{ direction: "rtl" }}
            >
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }} align="right">
                    Candidate
                  </TableCell>
                  <TableCell sx={{ fontWeight: 700 }} align="right">
                    Position
                  </TableCell>
                  <TableCell sx={{ fontWeight: 700 }} align="right">
                    Date
                  </TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {candsStore.candsReportData?.map((row, i) => (
                  <TableRow
                    hover
                    key={i}
                    sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
                  >
                    <TableCell align="right">
                      <Button
                        sx={{ textTransform: "none" }}
                        onClick={() => handleCandClick(row)}
                      >
                        {`${row.firstName} ${row.lastName}`}
                      </Button>
                    </TableCell>
                    <TableCell align="right">
                      <Button
                        sx={{ textTransform: "none" }}
                        onClick={() =>
                          positionsStore.positionClick(row.positionId!, true)
                        }
                      >
                        {`${
                          row.positionName
                        } - ${customersContactsStore.findCustomerName(
                          row.customerId || 0
                        )}`}
                      </Button>
                    </TableCell>
                    <TableCell align="right">
                      {row.stageDate &&
                        format(new Date(row.stageDate), "MMM d, yyyy")}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>
      </Grid>
      <Grid item xs={12} lg={12} m={1} mt={0}>
        <h5> {totalRows}</h5>
      </Grid>
    </Grid>
  );
});
