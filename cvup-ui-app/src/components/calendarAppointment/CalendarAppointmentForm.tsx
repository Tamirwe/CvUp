import {
  Button,
  FormControl,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
} from "@mui/material";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { TimePicker } from "@mui/x-date-pickers/TimePicker";
import { observer } from "mobx-react";
import { useEffect, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { IAppointment, IEmailsAddress } from "../../models/GeneralModels";
import { EmailsToControl } from "../email/EmailsToControl";

interface IProps {
  onClose: () => void;
}

const durationsList: IIdName[] = [
  { id: 15, name: "15 minutes" },
  { id: 30, name: "30 minutes" },
  { id: 45, name: "45 minutes" },
  { id: 60, name: "1 hour" },
  { id: 90, name: "1.5 hours" },
  { id: 120, name: "2 hours" },
];

const newAppointment: IAppointment = {
  id: 0,
  subject: "",
  candidates: [],
  contacts: [],
  durationMinutes: 60,
  location: "",
  notes: "",
};

export const CalendarAppointmentForm = observer(({ onClose }: IProps) => {
  const { candsStore, customersContactsStore, positionsStore } = useStore();
  const [formModel, setFormModel] = useState<IAppointment>(newAppointment);
  const [submitError, setSubmitError] = useState("");
  const [isSubjectEdited, setIsSubjectEdited] = useState(false);

  const [candsOptionsList, setCandsOptionsList] = useState<IEmailsAddress[]>(
    []
  );
  const [candsToList, setCandsToList] = useState<IEmailsAddress[]>([]);
  const [contactsOptionsList, setContactsOptionsList] = useState<
    IEmailsAddress[]
  >([]);
  const [contactsToList, setContactsToList] = useState<IEmailsAddress[]>([]);

  // Same as CandidateEmailSender - the displayed candidate is the option/value
  const addCandidateEmail = () => {
    if (candsStore.candDisplay) {
      const candName = `${candsStore.candDisplay?.firstName?.trim() || ""} ${
        candsStore.candDisplay?.lastName?.trim() || ""
      }`;

      const candsList = [
        {
          id: 1,
          Address: candsStore.candDisplay?.email || "",
          Name: candName.trim() ? candName : candsStore.candDisplay?.email,
        },
      ];

      setCandsOptionsList(candsList);
      setCandsToList(candsList);
    } else {
      setCandsOptionsList([]);
      setCandsToList([]);
    }
  };

  // Same as ContactEmailSender - the position contacts are auto selected,
  // out of the contacts of the position customer
  const fillContactsList = () => {
    let contacts = customersContactsStore.contactsListSorted;

    if (positionsStore.candDisplayPosition) {
      contacts = customersContactsStore.contactsListSorted.filter(
        (x) => x.customerId === positionsStore.candDisplayPosition?.customerId
      );
    }

    const optionsList = contacts.map((item, ind) => {
      const emailAddress: IEmailsAddress = {
        id: ind,
        Name: `${item.firstName || ""} ${item.lastName || ""} - ${
          item.customerName || ""
        }`,
        Address: item.email || "",
      };

      return emailAddress;
    });

    setContactsOptionsList(optionsList || []);

    const toList = positionsStore.candDisplayPosition?.contactsIds?.map((id) => {
      const contact = customersContactsStore.getContactById(id);

      return {
        Address: contact.email || "",
        Name: `${contact.firstName || ""} ${contact.lastName || ""} - ${
          contact.customerName || ""
        }`,
      };
    });

    setContactsToList(toList || []);
  };

  // The displayed candidate is auto selected, and follows the displayed cv
  useEffect(() => {
    addCandidateEmail();
  }, [candsStore.candDisplay]); // eslint-disable-line react-hooks/exhaustive-deps

  const contactsCount = customersContactsStore.contactsListSorted.length;
  const candDisplayPosition = positionsStore.candDisplayPosition;

  useEffect(() => {
    fillContactsList();
  }, [contactsCount, candDisplayPosition]); // eslint-disable-line react-hooks/exhaustive-deps

  // A different position / candidate is a different appointment,
  // so the generated subject takes over again
  useEffect(() => {
    setIsSubjectEdited(false);
  }, [candDisplayPosition, candsStore.candDisplay]);

  // Keep the subject in sync with the displayed position, until it is edited
  useEffect(() => {
    if (!isSubjectEdited) {
      setFormModel((currentProps) => ({
        ...currentProps,
        subject: `ראיון עבודה למשרת ${candDisplayPosition?.name || ""}`,
      }));
    }
  }, [candDisplayPosition?.name, isSubjectEdited]);

  const updateField = (field: keyof IAppointment, value: any) => {
    setSubmitError("");
    setFormModel((currentProps) => ({ ...currentProps, [field]: value }));
  };

  const submitForm = async () => {
    // TODO: call the appointments store
    console.log({
      ...formModel,
      candidates: candsToList,
      contacts: contactsToList,
    });
    onClose();
  };

  return (
    <form noValidate spellCheck="false" >
      <LocalizationProvider dateAdapter={AdapterDateFns}>
        <Grid container spacing={2} >
          <Grid item xs={12} mt={2}>
            <TextField
              sx={{ direction: "rtl" }}
              fullWidth
              type="text"
              id="subjectInp"
              label="Subject"
              variant="outlined"
              value={formModel.subject}
              onChange={(e) => {
                setIsSubjectEdited(true);
                updateField("subject", e.target.value);
              }}
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <EmailsToControl
              id="appointmentCands"
              label="Candidate"
              placeholder="Candidate"
              optionsList={candsOptionsList}
              listValues={candsToList}
              onChange={(vals) => {
                setCandsToList(vals);
                setSubmitError("");
              }}
            />
          </Grid>

          <Grid item xs={12} sm={6}>
            <EmailsToControl
              id="appointmentContacts"
              label="Contact"
              placeholder="Contact person"
              optionsList={contactsOptionsList}
              listValues={contactsToList}
              onChange={(vals) => {
                setContactsToList(vals);
                setSubmitError("");
              }}
            />
          </Grid>

          <Grid item xs={12} sm={4}>
            <DatePicker
              label="Date"
              value={formModel.startDate || null}
              onChange={(newValue) => updateField("startDate", newValue)}
              slotProps={{
                textField: { fullWidth: true},
              }}
            />
          </Grid>

          <Grid item xs={12} sm={4}>
            <TimePicker
              label="Time"
              ampm={false}
              value={formModel.startTime || null}
              onChange={(newValue) => updateField("startTime", newValue)}
              slotProps={{
                textField: { fullWidth: true },
              }}
            />
          </Grid>

          <Grid item xs={12} sm={4}>
            <FormControl fullWidth >
              <InputLabel id="durationLabel">Duration</InputLabel>
              <Select
                labelId="durationLabel"
                id="durationSelect"
                label="Duration"
                value={formModel.durationMinutes.toString()}
                onChange={(e) =>
                  updateField("durationMinutes", parseInt(e.target.value))
                }
              >
                {durationsList.map((item) => {
                  return (
                    <MenuItem
                      key={item.id}
                      value={item.id}
                      sx={{ direction: "rtl" }}
                    >
                      {item.name}
                    </MenuItem>
                  );
                })}
              </Select>
            </FormControl>
          </Grid>

          <Grid item xs={12}>
            <TextField
              sx={{ direction: "rtl" }}
              fullWidth
              type="text"
              id="locationInp"
              label="Location"
              variant="outlined"
              value={formModel.location}
              onChange={(e) => updateField("location", e.target.value)}
            />
          </Grid>

          <Grid item xs={12}>
            <TextField
              sx={{ direction: "rtl" }}
              fullWidth
              multiline
              rows={4}
              type="text"
              id="notesInp"
              label="Notes"
              variant="outlined"
              value={formModel.notes}
              onChange={(e) => updateField("notes", e.target.value)}
            />
          </Grid>

          <Grid item xs={12}>
            <FormHelperText error sx={{ direction: "rtl" }}>
              {submitError}
            </FormHelperText>
          </Grid>

          <Grid item xs={12}>
            <Stack direction="row" justifyContent="flex-end" gap={1}>
              <Button
                variant="contained"
                color="secondary"
                onClick={() => submitForm()}
              >
                Save
              </Button>
              <Button
                variant="contained"
                color="secondary"
                onClick={() => onClose()}
              >
                Cancel
              </Button>
            </Stack>
          </Grid>
        </Grid>
      </LocalizationProvider>
    </form>
  );
});
