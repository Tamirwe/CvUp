import { useEffect, useMemo, useState } from "react";
import { observer } from "mobx-react";
import { useSearchParams } from "react-router-dom";
import {
  Box,
  Button,
  Divider,
  Grid,
  IconButton,
  InputAdornment,
  List,
  ListItemButton,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Tooltip,
  Typography,
} from "@mui/material";
import {
  MdOutlineSearch,
  MdOutlinePersonAddAlt,
  MdOutlineSettings,
  MdOutlineSend,
  MdOutlineDelete,
  MdOutlineSave,
} from "react-icons/md";
import { isMobile } from "react-device-detect";
import { useStore } from "../Hooks/useStore";
import { IContact, ICustomer } from "../models/GeneralModels";
import { AlertConfirmDialogEnum } from "../models/GeneralEnums";
import { ContractTemplatesDialog } from "../components/contracts/ContractTemplatesDialog";

const CARD_BG = "#f7f9fa";
const CARD_BORDER = "1px solid #dfe4ea";
const INK = "#2f3b45";
const INK_SECONDARY = "#5d6b77";
const INK_MUTED = "#8a97a3";
const ACCENT = "#4d7c9e";

const EMPTY_CUSTOMER: ICustomer = { id: 0, name: "", address: "", descr: "" };
const EMPTY_CONTACT: IContact = {
  id: 0,
  firstName: "",
  lastName: "",
  customerId: 0,
  email: "",
  phone: "",
  role: "",
};

// Email templates are shared with the candidate/contact senders, so the send
// form takes only the ones tagged for contracts.
const CONTRACT_TEMPLATE_TYPE = "contract";

const Panel = ({
  title,
  subtitle,
  action,
  children,
}: {
  title: string;
  subtitle?: string;
  action?: React.ReactNode;
  children: React.ReactNode;
}) => {
  return (
    <Paper
      elevation={0}
      sx={{
        border: CARD_BORDER,
        borderRadius: 2,
        backgroundColor: CARD_BG,
        height: "100%",
      }}
    >
      <Stack
        direction="row"
        alignItems="center"
        sx={{ padding: "0.6rem 1rem" }}
        spacing={1}
      >
        <Typography
          sx={{ fontSize: "0.95rem", fontWeight: 600, flexShrink: 0 }}
          color={INK}
        >
          {title}
        </Typography>
        {/* Shares the title's row and takes the slack, so the action stays
            pinned right whether or not a panel has a subtitle. */}
        <Typography
          sx={{ flexGrow: 1, fontSize: "0.8rem" }}
          color={INK_SECONDARY}
        >
          {subtitle}
        </Typography>
        {action}
      </Stack>
      <Divider />
      <Box sx={{ padding: "1rem" }}>{children}</Box>
    </Paper>
  );
};

export const AdminCustomers = observer(() => {
  const { customersContactsStore, customerContractsStore, candsStore, generalStore } =
    useStore();
  const [searchParams, setSearchParams] = useSearchParams();

  const [search, setSearch] = useState("");
  const [customer, setCustomer] = useState<ICustomer>({ ...EMPTY_CUSTOMER });
  const [contact, setContact] = useState<IContact>({ ...EMPTY_CONTACT });
  const [templateId, setTemplateId] = useState(0);
  const [emailTemplateId, setEmailTemplateId] = useState(0);
  const [subject, setSubject] = useState("");
  const [body, setBody] = useState("");
  const [descr, setDescr] = useState("");
  const [isTemplatesOpen, setIsTemplatesOpen] = useState(false);

  useEffect(() => {
    customersContactsStore.getCustomersList();
    customersContactsStore.getContactsList();
    customerContractsStore.getContractTemplates();
    candsStore.getEmailTemplates();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  // The dashboard widget links here with ?customerId=, so a contract opens on
  // the customer it belongs to.
  useEffect(() => {
    const id = Number(searchParams.get("customerId"));

    if (id && customersContactsStore.customersList.length) {
      selectCustomer(id);
      searchParams.delete("customerId");
      setSearchParams(searchParams, { replace: true });
    }
  }, [searchParams, customersContactsStore.customersList]); // eslint-disable-line react-hooks/exhaustive-deps

  const customersList = useMemo(() => {
    const list = customersContactsStore.customersList.slice();
    const phrase = search.trim().toLowerCase();

    return phrase
      ? list.filter((x) => x.name.toLowerCase().includes(phrase))
      : list;
  }, [customersContactsStore.customersList, search]);

  const customerContacts = useMemo(() => {
    const list = customer.id
      ? customersContactsStore.getContactsByCustomerId(customer.id)
      : [];

    // A contact saved a moment ago must be in the list, or the dropdown has no
    // option matching the selection and silently shows nothing. The shared
    // contacts list is refreshed on save, so this only covers the gap when it
    // has not come back yet.
    if (contact.id && !list.some((x) => x.id === contact.id)) {
      list.push(contact);
    }

    return list;
  }, [customersContactsStore.contactsList, customer.id, contact]); // eslint-disable-line react-hooks/exhaustive-deps

  const contractEmailTemplates = useMemo(() => {
    return (candsStore.emailTemplates ?? []).filter(
      (x) => x.templateType === CONTRACT_TEMPLATE_TYPE
    );
  }, [candsStore.emailTemplates]);

  const selectCustomer = async (customerId: number) => {
    const selected = customersContactsStore.customersList.find(
      (x) => x.id === customerId
    );

    if (!selected) {
      return;
    }

    setCustomer({ ...EMPTY_CUSTOMER, ...selected });
    setDescr("");

    // The oldest contact is the main one -- there is no flag for it.
    const contacts = customersContactsStore.getContactsByCustomerId(customerId);
    const mainContact = contacts.slice().sort((a, b) => a.id - b.id)[0];
    setContact(mainContact ? { ...mainContact } : { ...EMPTY_CONTACT, customerId });
  };

  const handleNewCustomer = () => {
    setCustomer({ ...EMPTY_CUSTOMER });
    setContact({ ...EMPTY_CONTACT });
    setDescr("");
  };

  const handleSaveCustomer = async () => {
    if (!customer.name.trim()) {
      generalStore.alertSnackbar("error", "A customer name is needed.");
      return;
    }

    const res = customer.id
      ? await customersContactsStore.updateCustomer(customer)
      : await customersContactsStore.addCustomer(customer);

    if (!res.isSuccess) {
      generalStore.alertSnackbar("error", "The customer could not be saved.");
      return;
    }

    const customerId = res.data?.id ?? customer.id;
    await customersContactsStore.getCustomersList();
    setCustomer({ ...customer, id: customerId });
    setContact({ ...contact, customerId });
    generalStore.alertSnackbar("success", "Customer saved.");
  };

  const handleSaveContact = async () => {
    if (!customer.id) {
      generalStore.alertSnackbar("error", "Save the customer first.");
      return;
    }

    if (!contact.email.trim()) {
      generalStore.alertSnackbar("error", "A contact email is needed.");
      return;
    }

    const data = { ...contact, customerId: customer.id };
    const res = contact.id
      ? await customersContactsStore.updateContact(data)
      : await customersContactsStore.addContact(data);

    if (!res.isSuccess) {
      generalStore.alertSnackbar("error", "The contact could not be saved.");
      return;
    }

    await customersContactsStore.getContactsList();

    // A new contact comes back with the id the database gave it. Without
    // adopting it the dropdown falls back to "New contact" and the send is
    // refused for having no contact -- right after one was created.
    const savedId =
      (res.data as IContact | undefined)?.id ||
      customersContactsStore
        .getContactsByCustomerId(customer.id)
        .find((x) => x.email === data.email)?.id ||
      contact.id;

    setContact({ ...data, id: savedId });
    generalStore.alertSnackbar("success", "Contact saved.");
  };

  const handleDeleteContact = async () => {
    if (!contact.id) {
      return;
    }

    const isOk = await generalStore.alertConfirmDialog(
      AlertConfirmDialogEnum.Confirm,
      "Delete contact",
      `Delete ${contact.firstName} ${contact.lastName}?`
    );

    if (!isOk) {
      return;
    }

    await customersContactsStore.deleteContact(contact.id);
    await customersContactsStore.getContactsList();
    setContact({ ...EMPTY_CONTACT, customerId: customer.id });
  };

  const handleSelectEmailTemplate = (id: number) => {
    setEmailTemplateId(id);
    const template = contractEmailTemplates.find((x) => x.id === id);

    if (template) {
      setSubject(template.subject);
      setBody(template.body);
    }
  };

  const handleSend = async () => {
    if (!customer.id || !contact.id) {
      generalStore.alertSnackbar("error", "Save the customer and contact first.");
      return;
    }

    if (!templateId) {
      generalStore.alertSnackbar("error", "Choose a contract template.");
      return;
    }

    const res = await customerContractsStore.sendContract({
      customerId: customer.id,
      contactId: contact.id,
      contractTemplateId: templateId,
      descr,
      subject,
      body,
    });

    if (res.isSuccess) {
      generalStore.alertSnackbar("success", `Sent to ${contact.email}.`);
      setDescr("");
    } else {
      generalStore.alertSnackbar("error", "The contract could not be sent.");
    }
  };

  return (
    <Box sx={{ padding: isMobile ? "0.75rem" : "1.25rem 1.5rem 2.5rem" }}>
      <ContractTemplatesDialog
        isOpen={isTemplatesOpen}
        onClose={() => {
          setIsTemplatesOpen(false);
          customerContractsStore.getContractTemplates();
        }}
      />

      <Stack
        direction={isMobile ? "column" : "row"}
        alignItems={isMobile ? "flex-start" : "flex-end"}
        spacing={1.5}
        sx={{ marginBottom: 2 }}
      >
        <Box sx={{ flexGrow: 1 }}>
          <Typography sx={{ fontSize: "1.5rem", fontWeight: 700 }} color={INK}>
            Customers
          </Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Button
            size="small"
            variant="outlined"
            startIcon={<MdOutlinePersonAddAlt />}
            onClick={handleNewCustomer}
          >
            New customer
          </Button>
          <Button
            size="small"
            variant="outlined"
            startIcon={<MdOutlineSettings />}
            onClick={() => setIsTemplatesOpen(true)}
          >
            Templates
          </Button>
        </Stack>
      </Stack>

      <Grid container spacing={2}>
        <Grid item xs={12} md={4} lg={3}>
          <Panel title="Customers">
            <TextField
              size="small"
              fullWidth
              placeholder="Search"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <MdOutlineSearch />
                  </InputAdornment>
                ),
              }}
            />
            <List
              dense
              sx={{
                marginTop: 1,
                maxHeight: isMobile ? "14rem" : "28rem",
                overflowY: "auto",
              }}
            >
              {customersList.map((item) => (
                <ListItemButton
                  key={item.id}
                  selected={item.id === customer.id}
                  onClick={() => selectCustomer(item.id)}
                  sx={{ borderRadius: 1 }}
                >
                  <Typography sx={{ fontSize: "0.9rem" }}>{item.name}</Typography>
                </ListItemButton>
              ))}
              {customersList.length === 0 && (
                <Typography sx={{ fontSize: "0.85rem" }} color={INK_MUTED}>
                  No customers.
                </Typography>
              )}
            </List>
          </Panel>
        </Grid>

        <Grid item xs={12} md={8} lg={5}>
          <Stack spacing={2}>
            <Panel
              title={customer.id ? "Customer" : "New customer"}
              action={
                <Button
                  size="small"
                  startIcon={<MdOutlineSave />}
                  onClick={handleSaveCustomer}
                  sx={{ textTransform: "none", color: ACCENT }}
                >
                  Save
                </Button>
              }
            >
              <Stack spacing={1.5}>
                <TextField
                  label="Name"
                  size="small"
                  value={customer.name}
                  onChange={(e) =>
                    setCustomer({ ...customer, name: e.target.value })
                  }
                />
                <TextField
                  label="Address"
                  size="small"
                  value={customer.address ?? ""}
                  onChange={(e) =>
                    setCustomer({ ...customer, address: e.target.value })
                  }
                />
                <TextField
                  label="Description"
                  size="small"
                  multiline
                  minRows={2}
                  value={customer.descr ?? ""}
                  onChange={(e) =>
                    setCustomer({ ...customer, descr: e.target.value })
                  }
                />
              </Stack>
            </Panel>

            <Panel
              title="Contact"
              action={
                <Stack direction="row" spacing={0.5} alignItems="center">
                  {contact.id > 0 && (
                    <Tooltip title="Delete contact">
                      <IconButton size="small" onClick={handleDeleteContact}>
                        <MdOutlineDelete />
                      </IconButton>
                    </Tooltip>
                  )}
                  <Button
                    size="small"
                    startIcon={<MdOutlineSave />}
                    onClick={handleSaveContact}
                    sx={{ textTransform: "none", color: ACCENT }}
                  >
                    Save
                  </Button>
                </Stack>
              }
            >
              <Stack spacing={1.5}>
                <TextField
                  select
                  size="small"
                  label="Contact"
                  value={contact.id}
                  onChange={(e) => {
                    const id = Number(e.target.value);
                    const selected = customerContacts.find((x) => x.id === id);
                    setContact(
                      selected
                        ? { ...selected }
                        : { ...EMPTY_CONTACT, customerId: customer.id }
                    );
                  }}
                >
                  <MenuItem value={0}>New contact</MenuItem>
                  {customerContacts.map((item) => (
                    <MenuItem key={item.id} value={item.id}>
                      {`${item.firstName} ${item.lastName ?? ""}`.trim()}
                    </MenuItem>
                  ))}
                </TextField>
                <Stack direction={isMobile ? "column" : "row"} spacing={1.5}>
                  <TextField
                    label="First name"
                    size="small"
                    fullWidth
                    value={contact.firstName}
                    onChange={(e) =>
                      setContact({ ...contact, firstName: e.target.value })
                    }
                  />
                  <TextField
                    label="Last name"
                    size="small"
                    fullWidth
                    value={contact.lastName ?? ""}
                    onChange={(e) =>
                      setContact({ ...contact, lastName: e.target.value })
                    }
                  />
                </Stack>
                <TextField
                  label="Email"
                  size="small"
                  value={contact.email}
                  onChange={(e) =>
                    setContact({ ...contact, email: e.target.value })
                  }
                />
                <Stack direction={isMobile ? "column" : "row"} spacing={1.5}>
                  <TextField
                    label="Phone"
                    size="small"
                    fullWidth
                    value={contact.phone ?? ""}
                    onChange={(e) =>
                      setContact({ ...contact, phone: e.target.value })
                    }
                  />
                  <TextField
                    label="Role"
                    size="small"
                    fullWidth
                    value={contact.role ?? ""}
                    onChange={(e) =>
                      setContact({ ...contact, role: e.target.value })
                    }
                  />
                </Stack>
              </Stack>
            </Panel>
          </Stack>
        </Grid>

        {/* Grid items stretch to the tallest in the row, which left the
            contract box with dead space under the Send button whenever the
            customer+contact column was taller. This one ends at its content. */}
        <Grid item xs={12} lg={4} sx={{ alignSelf: "flex-start" }}>
          <Panel
            title="Contract"
            subtitle=" - Send a contract for signing, as a Word document."
          >
            <Stack spacing={1.5}>
              {/* Kept on the contract row rather than the email: it is a note
                  about this send, not part of what the customer receives. */}
              <TextField
                label="Remarks - not sent to customer"
                size="small"
                value={descr}
                onChange={(e) => setDescr(e.target.value)}
              />

              <TextField
                select
                size="small"
                label="Contract template"
                value={templateId}
                onChange={(e) => setTemplateId(Number(e.target.value))}
                helperText={
                  customerContractsStore.sendableTemplates.length === 0
                    ? "No template file found. Upload one under Templates."
                    : " "
                }
              >
                <MenuItem value={0}>Choose a template</MenuItem>
                {customerContractsStore.sendableTemplates.map((item) => (
                  <MenuItem key={item.id} value={item.id}>
                    {item.name}
                  </MenuItem>
                ))}
              </TextField>

              <TextField
                select
                size="small"
                label="Email template"
                value={emailTemplateId}
                onChange={(e) =>
                  handleSelectEmailTemplate(Number(e.target.value))
                }
                helperText={
                  contractEmailTemplates.length === 0
                    ? "No email template is tagged as 'contract'."
                    : " "
                }
              >
                <MenuItem value={0}>Choose a template</MenuItem>
                {contractEmailTemplates.map((item) => (
                  <MenuItem key={item.id} value={item.id}>
                    {item.name}
                  </MenuItem>
                ))}
              </TextField>

              <TextField
                label="Subject"
                size="small"
                value={subject}
                onChange={(e) => setSubject(e.target.value)}
              />
              <TextField
                label="Body"
                size="small"
                multiline
                minRows={4}
                value={body}
                onChange={(e) => setBody(e.target.value)}
              />

              {/* Last in the box: the form above is read top to bottom and
                  sending is what it ends in. */}
              <Button
                variant="contained"
                startIcon={<MdOutlineSend />}
                onClick={handleSend}
                sx={{ alignSelf: "flex-end" }}
              >
                Send
              </Button>
            </Stack>
          </Panel>
        </Grid>
      </Grid>
    </Box>
  );
});
