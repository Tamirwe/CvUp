import {
  Box,
  Collapse,
  Dialog,
  DialogContent,
  IconButton,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { format } from "date-fns";
import { observer } from "mobx-react";
import { Fragment, useEffect, useState } from "react";
import { MdKeyboardArrowDown, MdKeyboardArrowUp } from "react-icons/md";
import { useStore } from "../../Hooks/useStore";
import { IContractSend } from "../../models/GeneralModels";
import { BootstrapDialogTitle } from "../dialog/BootstrapDialogTitle";

interface IProps {
  isOpen: boolean;
  onClose: () => void;
}

const COLUMNS_COUNT = 6;

const matchesSearch = (item: IContractSend, search: string) =>
  [
    item.companyName,
    item.contactName,
    item.emailAddress,
    item.contractTemplateName,
    item.emailSubject,
  ].some((value) => value?.toLowerCase().includes(search));

export const ContractSendsDialog = observer(({ isOpen, onClose }: IProps) => {
  const { contractTemplatesStore, generalStore } = useStore();
  const [search, setSearch] = useState("");
  const [openRowId, setOpenRowId] = useState(0);

  useEffect(() => {
    (async () => {
      const res = await contractTemplatesStore.getContractSends();

      if (!res.isSuccess) {
        generalStore.alertSnackbar("error", "Failed to load the sent contracts");
      }
    })();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const searchText = search.trim().toLowerCase();
  const sendsList = searchText
    ? contractTemplatesStore.contractSendsList.filter((x) =>
        matchesSearch(x, searchText)
      )
    : contractTemplatesStore.contractSendsList;

  return (
    <Dialog open={isOpen} onClose={onClose} fullWidth maxWidth={"lg"}>
      <BootstrapDialogTitle id="contract-sends-title" onClose={onClose}>
        Sent Contracts
      </BootstrapDialogTitle>
      <DialogContent>
        <TextField
          fullWidth
          size="small"
          id="contractSendsSearch"
          label="Search company, contact, email or contract"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          sx={{ mt: 1, mb: 2 }}
        />
        <TableContainer sx={{ maxHeight: "65vh" }}>
          <Table stickyHeader size="small" aria-label="Sent contracts">
            <TableHead>
              <TableRow>
                <TableCell padding="checkbox" />
                <TableCell>Date sent</TableCell>
                <TableCell>Company</TableCell>
                <TableCell>Contact</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>Contract</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {sendsList.map((item) => {
                const isRowOpen = item.id === openRowId;

                return (
                  <Fragment key={item.id}>
                    <TableRow
                      hover
                      sx={{ "& > td": { borderBottom: isRowOpen ? "unset" : undefined } }}
                    >
                      <TableCell padding="checkbox">
                        <IconButton
                          size="small"
                          aria-label={isRowOpen ? "Hide email" : "Show email"}
                          aria-expanded={isRowOpen}
                          onClick={() => setOpenRowId(isRowOpen ? 0 : item.id)}
                        >
                          {isRowOpen ? <MdKeyboardArrowUp /> : <MdKeyboardArrowDown />}
                        </IconButton>
                      </TableCell>
                      <TableCell sx={{ whiteSpace: "nowrap" }}>
                        {format(new Date(item.dateSent), "MMM d, yyyy HH:mm")}
                      </TableCell>
                      <TableCell sx={{ direction: "rtl", textAlign: "left" }}>
                        {item.companyName}
                      </TableCell>
                      <TableCell sx={{ direction: "rtl", textAlign: "left" }}>
                        {item.contactName}
                      </TableCell>
                      <TableCell>{item.emailAddress}</TableCell>
                      <TableCell>{item.contractTemplateName}</TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell colSpan={COLUMNS_COUNT} sx={{ py: 0 }}>
                        <Collapse in={isRowOpen} timeout="auto" unmountOnExit>
                          <Box sx={{ py: 2, direction: "rtl" }}>
                            <Typography variant="subtitle2" gutterBottom>
                              {item.emailSubject}
                            </Typography>
                            <div
                              dangerouslySetInnerHTML={{
                                __html: item.emailBody || "",
                              }}
                            />
                          </Box>
                        </Collapse>
                      </TableCell>
                    </TableRow>
                  </Fragment>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
        {sendsList.length === 0 && (
          <Typography color="text.secondary" sx={{ pt: 2, textAlign: "center" }}>
            {searchText ? "No sent contracts match the search" : "No contracts sent yet"}
          </Typography>
        )}
      </DialogContent>
    </Dialog>
  );
});
