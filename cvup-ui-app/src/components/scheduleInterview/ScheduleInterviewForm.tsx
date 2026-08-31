import {
  Button,
  Checkbox,
  FormControl,
  FormControlLabel,
  FormHelperText,
  Grid,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { MdKeyboardArrowDown, MdKeyboardArrowUp } from "react-icons/md";
import { AdapterDateFns } from "@mui/x-date-pickers/AdapterDateFns";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { TimePicker } from "@mui/x-date-pickers/TimePicker";
import { observer } from "mobx-react";
import { useEffect, useRef, useState } from "react";
import { useStore } from "../../Hooks/useStore";
import { IIdName } from "../../models/AuthModels";
import { AlertConfirmDialogEnum } from "../../models/GeneralEnums";
import {
  IScheduleInterview,
  IScheduleInterviewSend,
  IEmailsAddress,
} from "../../models/GeneralModels";
import { EmailsToControl } from "../email/EmailsToControl";

interface IProps {
  isOpen: boolean;
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

// Tomorrow, unless it falls on the weekend - then the upcoming Sunday
const defaultStartDate = () => {
  const startDate = new Date();
  startDate.setHours(0, 0, 0, 0);
  startDate.setDate(startDate.getDate() + 1);

  while (startDate.getDay() === 5 || startDate.getDay() === 6) {
    startDate.setDate(startDate.getDate() + 1);
  }

  return startDate;
};

// Interviews are set to 11:00 unless another time is picked
const defaultStartTime = () => {
  const startTime = new Date();
  startTime.setHours(11, 0, 0, 0);

  return startTime;
};

const newScheduleInterview: IScheduleInterview = {
  id: 0,
  subject: "",
  candidates: [],
  contacts: [],
  durationMinutes: 60,
  location: "",
  notes: "",
};

export const ScheduleInterviewForm = observer(({ isOpen, onClose }: IProps) => {
  const { scheduleInterviewStore, candsStore, customersContactsStore, generalStore, positionsStore } =
    useStore();
  const [formModel, setFormModel] = useState<IScheduleInterview>({
    ...newScheduleInterview,
    startDate: defaultStartDate(),
    startTime: defaultStartTime(),
  });
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
  const [isOptionalOpen, setIsOptionalOpen] = useState(false);
  const [optionalToList, setOptionalToList] = useState<IEmailsAddress[]>([]);
  const [isAddMeAsOptional, setIsAddMeAsOptional] = useState(false);
  const [scheduleInterviewId, setScheduleInterviewId] = useState(0);

  // An interview that was loaded from the server keeps its own guests,
  // the position / candidate defaults must not run over them
  const isLoadedScheduleInterview = useRef(false);

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

      if (!isLoadedScheduleInterview.current) {
        setCandsToList(candsList);
      }
    } else {
      setCandsOptionsList([]);

      if (!isLoadedScheduleInterview.current) {
        setCandsToList([]);
      }
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

    if (!isLoadedScheduleInterview.current) {
      setContactsToList(toList || []);
    }
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

  const candidateId = candsStore.candDisplay?.candidateId;
  const positionId = candDisplayPosition?.id;

  // The api serializes with camelCase, the emails control works with Name / Address
  const toGuests = (guests?: IEmailsAddress[]): IEmailsAddress[] => {
    return (guests || []).map((guest: any) => {
      return {
        Name: guest.Name ?? guest.name,
        Address: guest.Address ?? guest.address,
      };
    });
  };

  // An interview that is already set for this candidate on this position
  // is edited by the form, instead of being set again
  const loadScheduleInterview = async () => {
    const res = await scheduleInterviewStore.getScheduleInterview(positionId, candidateId);
    const data = res.isSuccess ? res.data : null;

    setSubmitError("");

    if (data) {
      const start = new Date(data.start);

      isLoadedScheduleInterview.current = true;
      setScheduleInterviewId(data.id || 0);
      setFormModel({
        id: data.id || 0,
        subject: data.subject,
        candidates: [],
        contacts: [],
        durationMinutes: data.durationMinutes,
        startDate: start,
        startTime: start,
        location: data.location || "",
        notes: data.notes || "",
      });
      setIsSubjectEdited(true);
      const optionalGuests = toGuests(data.optionalParticipants);

      setCandsToList(toGuests(data.candidates));
      setContactsToList(toGuests(data.contacts));
      setOptionalToList(optionalGuests);
      setIsOptionalOpen(optionalGuests.length > 0);
    } else {
      isLoadedScheduleInterview.current = false;
      setScheduleInterviewId(0);
      setFormModel({
        ...newScheduleInterview,
        startDate: defaultStartDate(),
        startTime: defaultStartTime(),
      });
      setIsSubjectEdited(false);
      setOptionalToList([]);
      setIsOptionalOpen(false);
      setIsAddMeAsOptional(false);
      addCandidateEmail();
      fillContactsList();
    }
  };

  useEffect(() => {
    if (isOpen) {
      loadScheduleInterview();
    }
  }, [isOpen, positionId, candidateId]); // eslint-disable-line react-hooks/exhaustive-deps

  // A different position / candidate is a different interview,
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

  const updateField = (field: keyof IScheduleInterview, value: any) => {
    setSubmitError("");
    setFormModel((currentProps) => ({ ...currentProps, [field]: value }));
  };

  const scheduleInterviewStart = () => {
    if (!formModel.startDate || !formModel.startTime) {
      return null;
    }

    const start = new Date(formModel.startDate);
    start.setHours(
      formModel.startTime.getHours(),
      formModel.startTime.getMinutes(),
      0,
      0
    );

    return start;
  };

  const buildScheduleInterview = (start: Date): IScheduleInterviewSend => {
    return {
      id: scheduleInterviewId,
      positionId: positionId,
      candidateId: candidateId,
      subject: formModel.subject,
      start: start.toISOString(),
      durationMinutes: formModel.durationMinutes,
      location: formModel.location,
      notes: formModel.notes,
      candidates: candsToList,
      contacts: contactsToList,
      optionalParticipants: optionalToList,
      isAddMeAsOptionalGuest: isAddMeAsOptional,
    };
  };

  const submitForm = async () => {
    if (!candsToList.length) {
      setSubmitError("Select at least one candidate");
      return;
    }

    if (!contactsToList.length) {
      setSubmitError("Select at least one contact");
      return;
    }

    if ((formModel.location || "").trim().length < 5) {
      setSubmitError("Location must be at least 5 characters");
      return;
    }

    const start = scheduleInterviewStart();

    if (!start) {
      setSubmitError("Select a date and a time");
      return;
    }

    const scheduleInterview = buildScheduleInterview(start);

    const res = scheduleInterviewId
      ? await scheduleInterviewStore.updateScheduleInterview(scheduleInterview)
      : await scheduleInterviewStore.addScheduleInterview(scheduleInterview);

    if (res.isSuccess) {
      generalStore.alertSnackbar(
        "success",
        scheduleInterviewId
          ? "Interview updated"
          : "Interview scheduled"
      );
      onClose();
    } else {
      generalStore.alertSnackbar("error", "Failed to schedule the interview");
    }
  };

  const removeScheduleInterview = async () => {
    const isRemove = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Remove interview",
      "Are you sure you want to remove this interview? A cancellation will be sent to the guests."
    );

    if (!isRemove) {
      return;
    }

    const res = await scheduleInterviewStore.cancelScheduleInterview(
      buildScheduleInterview(scheduleInterviewStart() || new Date())
    );

    if (res.isSuccess) {
      generalStore.alertSnackbar("success", "Interview removed");
      onClose();
    } else {
      generalStore.alertSnackbar("error", "Failed to remove the interview");
    }
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

          <Grid item xs={12}>
            <EmailsToControl
              id="scheduleInterviewCands"
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

          <Grid item xs={12}>
            <EmailsToControl
              id="scheduleInterviewContacts"
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

          <Grid item xs={12}>
            <Stack
              direction="row"
              alignItems="center"
              gap={0.5}
              onClick={() => setIsOptionalOpen(!isOptionalOpen)}
              sx={{ cursor: "pointer", width: "fit-content", fontSize: "1.3rem" }}
            >
              <Typography>Optional participants</Typography>
              {isOptionalOpen ? <MdKeyboardArrowUp /> : <MdKeyboardArrowDown />}
            </Stack>

            {isOptionalOpen && (
              <Stack gap={1} sx={{ pt: 2 }}>
                <EmailsToControl
                  id="scheduleInterviewOptionalParticipants"
                  label="Optional participant"
                  placeholder="Optional participant"
                  optionsList={contactsOptionsList}
                  listValues={optionalToList}
                  onChange={(vals) => {
                    setOptionalToList(vals);
                    setSubmitError("");
                  }}
                />
                <FormControlLabel
                  control={
                    <Checkbox
                      id="isAddMeAsOptional"
                      checked={isAddMeAsOptional}
                      onChange={(e) => setIsAddMeAsOptional(e.target.checked)}
                    />
                  }
                  label="Add me as optional guest"
                />
              </Stack>
            )}
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
                {scheduleInterviewId
                  ? "Update interview"
                  : "Schedule interview"}
              </Button>
              {scheduleInterviewId > 0 && (
                <Button
                  variant="outlined"
                  color="error"
                  onClick={() => removeScheduleInterview()}
                >
                  Remove interview
                </Button>
              )}
              <Button
                variant="outlined"
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
