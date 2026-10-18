import { ReactNode, useEffect } from "react";
import { observer } from "mobx-react";
import { useNavigate } from "react-router-dom";
import { useStore } from "../Hooks/useStore";
import {
  Box,
  Button,
  Chip,
  Divider,
  Grid,
  MenuItem,
  Paper,
  Select,
  Skeleton,
  Stack,
  Typography,
} from "@mui/material";
import { isMobile } from "react-device-detect";
import {
  MdOutlineInsertChart,
  MdOutlineDescription,
  MdOutlineLink,
  MdOutlineAlarm,
  MdOutlineMailOutline,
  MdOutlineNotificationsNone,
  MdOutlineExtension,
  MdChevronRight,
} from "react-icons/md";

// A skeleton: the frame every future admin widget drops into -- reports, links,
// reminders, features, charts, messages and notifications each own a panel.
// Nothing here reads a store yet, and nothing shows invented figures: the value
// slots hold a dash and the plot areas hold a labelled outline, so the page is
// never mistaken for live data.
//
// Chart colours are deliberately unset. A categorical palette is chosen with
// the real series (and validated for colour-blind separation) rather than
// guessed here against placeholder boxes.

// A dim, low-glare set: the page sits on a soft grey-blue ground and the cards
// are off-white rather than #fff, so a screen full of empty panels does not
// read as a sheet of white. Ink is slate instead of black and the accent is a
// muted blue. The page ground itself is painted by AdminLayout.
const CARD_BG = "#f7f9fa";
const CARD_BORDER = "1px solid #dfe4ea";
const PLACEHOLDER_BG = "#e8ecf1";
const PLACEHOLDER_BORDER = "1px dashed #cfd6de";
const SKELETON_STRONG = "#dde2e8";
const SKELETON_SOFT = "#e6eaef";
const INK = "#2f3b45";
const INK_SECONDARY = "#5d6b77";
const INK_MUTED = "#8a97a3";
const ACCENT = "#4d7c9e";

interface IPanelProps {
  title: string;
  icon: ReactNode;
  action?: string;
  onAction?: () => void;
  children: ReactNode;
}

const Panel = ({ title, icon, action, onAction, children }: IPanelProps) => {
  return (
    <Paper
      elevation={0}
      sx={{
        border: CARD_BORDER,
        borderRadius: 2,
        backgroundColor: CARD_BG,
        height: "100%",
        display: "flex",
        flexDirection: "column",
      }}
    >
      <Stack
        direction="row"
        alignItems="center"
        spacing={1}
        sx={{ padding: "0.75rem 1rem" }}
      >
        <Box
          sx={{
            display: "flex",
            alignItems: "center",
            fontSize: "1.15rem",
            color: ACCENT,
          }}
        >
          {icon}
        </Box>
        <Typography
          sx={{ flexGrow: 1, fontSize: "0.95rem", fontWeight: 600 }}
          color={INK}
        >
          {title}
        </Typography>
        {action && (
          <Button
            size="small"
            onClick={onAction}
            endIcon={<MdChevronRight />}
            sx={{
              textTransform: "none",
              fontSize: "0.8rem",
              // Not the theme's bright primary -- it would be the loudest thing
              // on an otherwise quiet page.
              color: ACCENT,
            }}
          >
            {action}
          </Button>
        )}
      </Stack>
      <Divider />
      <Box sx={{ padding: "1rem", flexGrow: 1 }}>{children}</Box>
    </Paper>
  );
};

// The empty state of a panel that is not built yet: says what will live here
// instead of pretending to load something.
const Placeholder = ({
  label,
  height,
}: {
  label: string;
  height: number | string;
}) => {
  return (
    <Box
      sx={{
        height,
        border: PLACEHOLDER_BORDER,
        borderRadius: 2,
        backgroundColor: PLACEHOLDER_BG,
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 1,
        textAlign: "center",
      }}
    >
      <Typography sx={{ fontSize: "0.8rem" }} color={INK_MUTED}>
        {label}
      </Typography>
    </Box>
  );
};

// Rows of a list-shaped panel (reports, messages, notifications, reminders).
const PlaceholderRows = ({ rows }: { rows: number }) => {
  return (
    <Stack divider={<Divider />} spacing={0}>
      {Array.from({ length: rows }).map((_, i) => (
        <Stack
          key={i}
          direction="row"
          alignItems="center"
          spacing={1.5}
          sx={{ paddingY: 1.15 }}
        >
          <Skeleton
            animation={false}
            variant="circular"
            width={28}
            height={28}
            sx={{ flexShrink: 0, backgroundColor: SKELETON_STRONG }}
          />
          <Box sx={{ flexGrow: 1 }}>
            <Skeleton
              animation={false}
              variant="text"
              width={`${70 - i * 8}%`}
              sx={{ fontSize: "0.9rem", backgroundColor: SKELETON_STRONG }}
            />
            <Skeleton
              animation={false}
              variant="text"
              width={`${45 - i * 5}%`}
              sx={{ fontSize: "0.75rem", backgroundColor: SKELETON_SOFT }}
            />
          </Box>
          <Skeleton
            animation={false}
            variant="rounded"
            width={44}
            height={16}
            sx={{ flexShrink: 0, backgroundColor: SKELETON_STRONG }}
          />
        </Stack>
      ))}
    </Stack>
  );
};

// The last ten contracts the module sent. A row opens the module on that
// customer, which is what the ?customerId= parameter is for.
const LatestContracts = observer(() => {
  const { customerContractsStore } = useStore();
  const navigate = useNavigate();

  useEffect(() => {
    customerContractsStore.getLatestContracts();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  if (customerContractsStore.latestContracts.length === 0) {
    return (
      <Typography sx={{ fontSize: "0.85rem" }} color={INK_MUTED}>
        No contracts have been sent yet.
      </Typography>
    );
  }

  return (
    <Stack divider={<Divider />} spacing={0}>
      {customerContractsStore.latestContracts.map((contract) => (
        <Stack
          key={contract.id}
          direction="row"
          alignItems="center"
          spacing={1.5}
          onClick={() =>
            navigate(`/admin/customers?customerId=${contract.customerId}`)
          }
          sx={{
            paddingY: 1.15,
            cursor: "pointer",
            "&:hover": { backgroundColor: PLACEHOLDER_BG },
          }}
        >
          <Box sx={{ flexGrow: 1, minWidth: 0 }}>
            <Typography
              sx={{ fontSize: "0.9rem", fontWeight: 600 }}
              color={INK}
              noWrap
            >
              {contract.customerName}
            </Typography>
            <Typography sx={{ fontSize: "0.75rem" }} color={INK_MUTED} noWrap>
              {[contract.contractTemplateName, contract.contactName]
                .filter((x) => x)
                .join(" — ")}
            </Typography>
          </Box>
          <Typography
            sx={{ fontSize: "0.75rem", flexShrink: 0 }}
            color={INK_SECONDARY}
          >
            {contract.dateSent
              ? new Date(contract.dateSent).toLocaleDateString()
              : ""}
          </Typography>
        </Stack>
      ))}
    </Stack>
  );
});

// A headline number with no number yet. Kept plot-free on purpose -- a bare
// stat tile needs no hover layer.
const StatTile = ({ label }: { label: string }) => {
  return (
    <Paper
      elevation={0}
      sx={{
        border: CARD_BORDER,
        borderRadius: 2,
        backgroundColor: CARD_BG,
        padding: "0.9rem 1rem",
        height: "100%",
      }}
    >
      <Typography
        sx={{
          fontSize: "0.75rem",
          textTransform: "uppercase",
          letterSpacing: "0.04em",
        }}
        color={INK_SECONDARY}
      >
        {label}
      </Typography>
      <Typography
        sx={{ fontSize: "1.9rem", fontWeight: 700, lineHeight: 1.3 }}
        color={INK}
      >
        &mdash;
      </Typography>
      <Typography sx={{ fontSize: "0.75rem" }} color={INK_MUTED}>
        no data source yet
      </Typography>
    </Paper>
  );
};

export const AdminDashboard = observer(() => {
  const navigate = useNavigate();

  const statTiles = [
    "Candidates",
    "Open positions",
    "Interviews this week",
    "Active users",
  ];

  const quickLinks = [
    "Users",
    "Customers",
    "Positions",
    "Email templates",
    "Searches",
    "Folders",
  ];

  const features = [
    "Feature flags",
    "Integrations",
    "Imports",
    "Audit log",
    "Backups",
    "Permissions",
  ];

  return (
    <Box sx={{ padding: isMobile ? "0.75rem" : "1.25rem 1.5rem 2.5rem" }}>
      {/* Title, then the scope controls in a single row above the content. */}
      <Stack
        direction={isMobile ? "column" : "row"}
        alignItems={isMobile ? "flex-start" : "flex-end"}
        spacing={1.5}
        sx={{ marginBottom: 2 }}
      >
        <Box sx={{ flexGrow: 1 }}>
          <Typography sx={{ fontSize: "1.5rem", fontWeight: 700 }} color={INK}>
            Admin dashboard
          </Typography>
          <Typography sx={{ fontSize: "0.85rem" }} color={INK_SECONDARY}>
            Skeleton layout &mdash; panels are placeholders for upcoming
            features.
          </Typography>
        </Box>
        <Stack direction="row" spacing={1}>
          <Select size="small" value="30d" disabled sx={{ minWidth: 130 }}>
            <MenuItem value="30d">Last 30 days</MenuItem>
          </Select>
          <Select size="small" value="all" disabled sx={{ minWidth: 130 }}>
            <MenuItem value="all">All customers</MenuItem>
          </Select>
        </Stack>
      </Stack>

      <Grid container spacing={2}>
        {statTiles.map((label) => (
          <Grid key={label} item xs={6} md={3}>
            <StatTile label={label} />
          </Grid>
        ))}

        {/* Charts: a wide trend and a narrower breakdown. */}
        <Grid item xs={12} md={8}>
          <Panel title="Activity over time" icon={<MdOutlineInsertChart />}>
            <Placeholder label="Line chart — one series per stage" height={240} />
          </Panel>
        </Grid>
        <Grid item xs={12} md={4}>
          <Panel title="Breakdown" icon={<MdOutlineInsertChart />}>
            <Placeholder label="Bar chart — by position type" height={240} />
          </Panel>
        </Grid>

        {/* The one live panel so far: the contracts module's last ten sends. */}
        <Grid item xs={12} md={8}>
          <Panel
            title="Latest contracts"
            icon={<MdOutlineDescription />}
            action="Customers"
            onAction={() => navigate("/admin/customers")}
          >
            <LatestContracts />
          </Panel>
        </Grid>
        <Grid item xs={12} md={4}>
          <Panel title="Reminders" icon={<MdOutlineAlarm />}>
            <PlaceholderRows rows={4} />
          </Panel>
        </Grid>

        {/* Inbox-shaped panels. */}
        <Grid item xs={12} md={4}>
          <Panel title="Messages" icon={<MdOutlineMailOutline />}>
            <PlaceholderRows rows={3} />
          </Panel>
        </Grid>
        <Grid item xs={12} md={4}>
          <Panel title="Notifications" icon={<MdOutlineNotificationsNone />}>
            <PlaceholderRows rows={3} />
          </Panel>
        </Grid>
        <Grid item xs={12} md={4}>
          <Panel title="Quick links" icon={<MdOutlineLink />}>
            <Box sx={{ display: "flex", flexWrap: "wrap", gap: 1 }}>
              {quickLinks.map((link) => (
                <Chip
                  key={link}
                  label={link}
                  size="small"
                  variant="outlined"
                  clickable
                />
              ))}
            </Box>
          </Panel>
        </Grid>

        {/* Modules to be wired up, one tile each. */}
        <Grid item xs={12}>
          <Panel title="Features" icon={<MdOutlineExtension />}>
            <Grid container spacing={1.5}>
              {features.map((feature) => (
                <Grid key={feature} item xs={6} sm={4} md={2}>
                  <Placeholder label={feature} height={72} />
                </Grid>
              ))}
            </Grid>
          </Panel>
        </Grid>
      </Grid>
    </Box>
  );
});
