using System.Xml; 

namespace Ncaa14MixmatchViewer
{
    public partial class Ncaa14MixmatchViewer : Form
    {
        public Ncaa14MixmatchViewer()
        {
            InitializeComponent();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDialogMixmatch.ShowDialog() == DialogResult.OK)
            {
                DataGridView[] partsDataGrids = new[]{
                    helmetsDataGrid, jerseysDataGrid, pantsDataGrid, shoesDataGrid, socksDataGrid, glovesDataGrid
                };
                foreach (DataGridView dataGrid in partsDataGrids)
                {
                    dataGrid.Rows.Clear();
                }
                uniformListView.Items.Clear();

                // openDialogMixmatch.FileName
                XmlDocument xmlDoc = new();
                xmlDoc.Load(openDialogMixmatch.FileName);

                // Get all of the preset names
                string[] presetNames = new string[] { };
                XmlNodeList presets = xmlDoc.GetElementsByTagName("officialType");
                foreach (XmlNode preset in presets)
                {
                    string presetName = preset.Attributes["name"].Value;
                    presetNames.Append(preset.Attributes["name"].Value);
                    uniformListView.Items.Add(presetName);
                }

                // Get all of the parts
                XmlNodeList parts = xmlDoc.GetElementsByTagName("part");
                List<XmlNode> helmets = new List<XmlNode>();
                List<XmlNode> jerseys = new List<XmlNode>();
                List<XmlNode> pants = new List<XmlNode>();
                List<XmlNode> socks = new List<XmlNode>();
                List<XmlNode> shoes = new List<XmlNode>();
                List<XmlNode> gloves = new List<XmlNode>();

                foreach (XmlNode part in parts)
                {
                    if (part.Attributes["type"] == null) { continue; }
                    switch (part.Attributes["type"].Value)
                    {
                        case "helmet":
                            helmets.Add(part);
                            break;
                        case "jersey":
                            jerseys.Add(part);
                            break;
                        case "pants":
                            pants.Add(part);
                            break;
                        case "socks":
                            socks.Add(part);
                            break;
                        case "shoes":
                            shoes.Add(part);
                            break;
                        case "gloves":
                            gloves.Add(part);
                            break;
                    }
                }

                AddParts(helmets, helmetsDataGrid);
                AddParts(jerseys, jerseysDataGrid, true);
                AddParts(pants, pantsDataGrid);
                AddParts(socks, socksDataGrid);
                AddParts(shoes, shoesDataGrid);
                AddParts(gloves, glovesDataGrid);

                uniformListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                uniformListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void AddParts(List<XmlNode> parts, DataGridView dataGrid, bool isJersey = false)
        {
            foreach (XmlNode part in parts)
            {
                string name = part.Attributes["name"].Value;
                XmlNodeList childNodes = part.ChildNodes;
                List<string> presetsUsedIn = new List<string>();
                string bigfile = "";
                string scene = "";
                foreach (XmlNode child in childNodes)
                {
                    switch (child.Name)
                    {
                        case "bigfile":
                            bigfile = child.Attributes["name"].Value;
                            break;
                        case "scene":
                            scene = child.Attributes["name"].Value;
                            break;
                        case "official":
                            presetsUsedIn.Add(child.Attributes["name"].Value);
                            break;
                        default:
                            continue;
                    }
                }
                if (isJersey)
                {
                    dataGrid.Rows.Add(name, bigfile, scene, String.Join(',', presetsUsedIn), part.Attributes["shade"].Value);
                    continue;
                }

                dataGrid.Rows.Add(name, bigfile, scene, String.Join(',', presetsUsedIn));
            }
        }

        private void uniformListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataGridView[] partsDataGrids = new[]{
                helmetsDataGrid, jerseysDataGrid, pantsDataGrid, shoesDataGrid, socksDataGrid, glovesDataGrid
            };
            foreach (DataGridView dataGrid in partsDataGrids)
            {
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    row.Visible = true;
                }
            }

            if (uniformListView.SelectedItems.Count == 0) { return; }

            string presetName = uniformListView.SelectedItems[0].Text;

            foreach (DataGridView dataGrid in partsDataGrids) {
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    String[] presetNames = row.Cells[3].Value.ToString().Split(',');
                    if (!presetNames.Contains(presetName)) {
                        row.Visible = false;
                    }
                }
            }
        }
    }
}