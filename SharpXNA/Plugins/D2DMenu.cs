using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpXNA.Input;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SharpXNA.Plugins
{
    public class D2DMenu
    {
        public Color GroupColor = Color.Red, GroupSelectedColor = Color.Yellow, ItemColor = Color.White, ItemSelectedColor = Color.Yellow;

        internal List<Title> prefixes, suffixes;
        public List<Title> Prefixes { get { return prefixes; } }
        public List<Title> Suffixes { get { return suffixes; } }

        internal OrderedDictionary groups;
        public OrderedDictionary Groups { get { return groups; } }

        internal int groupSelected = -1, visibleItems = 0;
        public Group GroupSelected { get { if (groupSelected == -1) return null; else { try { return (Groups[groupSelected] as Group); } catch { groupSelected = -1; return null; } } } }

        public D2DMenu(string name) { groups = new OrderedDictionary(); prefixes = new List<Title>(1) { new Title(name, Color.White) }; suffixes = new List<Title>(); }
        public D2DMenu(string name, Color color) { groups = new OrderedDictionary(); prefixes = new List<Title>(1) { new Title(name, color) }; suffixes = new List<Title>(); }

        public void Update()
        {
            if (Keyboard.Pressed(Keyboard.Keys.Up))
            {
                if (GroupSelected == null) { if (Groups.Count > 0) groupSelected = (Groups.Count - 1); }
                else
                {
                    if (GroupSelected.State == Group.States.Expanded)
                    {
                        if (GroupSelected.ItemSelected == null)
                        {
                            if (Groups.Count == 1) GroupSelected.itemSelected = (GroupSelected.Items.Count - 1);
                            else
                            {
                                groupSelected--; if (groupSelected < 0) groupSelected = (Groups.Count - 1);
                                if ((GroupSelected.State == Group.States.Expanded) && (GroupSelected.Items.Count > 0)) GroupSelected.itemSelected = (GroupSelected.Items.Count - 1);
                            }
                        }
                        else GroupSelected.itemSelected--;
                    }
                    else
                    {
                        groupSelected--; if (groupSelected < 0) groupSelected = (Groups.Count - 1);
                        if ((GroupSelected.State == Group.States.Expanded) && (GroupSelected.Items.Count > 0)) GroupSelected.itemSelected = (GroupSelected.Items.Count - 1);
                    }
                }
            }
            if (Keyboard.Pressed(Keyboard.Keys.Down))
            {
                if (GroupSelected == null) { if (Groups.Count > 0) groupSelected = 0; }
                else
                {
                    if (GroupSelected.State == Group.States.Expanded)
                    {
                        if (GroupSelected.ItemSelected == null)
                        {
                            if (GroupSelected.Items.Count > 0) GroupSelected.itemSelected = 0;
                            else { groupSelected++; if (groupSelected >= GroupSelected.Items.Count) groupSelected = 0; }
                        }
                        else
                        {
                            GroupSelected.itemSelected++;
                            if (GroupSelected.itemSelected >= GroupSelected.Items.Count) { GroupSelected.itemSelected = -1; groupSelected++; if (groupSelected >= Groups.Count) groupSelected = 0; }
                        }
                    }
                    else { groupSelected++; if (groupSelected >= Groups.Count) groupSelected = 0; }
                }
            }
            if (Keyboard.Pressed(Keyboard.Keys.Left))
            {
                if (GroupSelected != null)
                {
                    if (GroupSelected.ItemSelected == null) { GroupSelected.state = (byte)((GroupSelected.state + 1) % 2); if (GroupSelected.State == Group.States.Expanded) visibleItems += GroupSelected.Items.Count; else visibleItems -= GroupSelected.Items.Count; }
                    else
                    {
                        if (GroupSelected.ItemSelected.OptionSelected == null) { if (GroupSelected.ItemSelected.Options.Count > 0) GroupSelected.ItemSelected.optionSelected = (GroupSelected.ItemSelected.Options.Count - 1); }
                        else { GroupSelected.ItemSelected.optionSelected--; if (GroupSelected.ItemSelected.optionSelected < 0) GroupSelected.ItemSelected.optionSelected = (GroupSelected.ItemSelected.Options.Count - 1); }
                    }
                }
            }
            if (Keyboard.Pressed(Keyboard.Keys.Right))
            {
                if (GroupSelected != null)
                {
                    if (GroupSelected.ItemSelected == null) { GroupSelected.state = (byte)((GroupSelected.state + 1) % 2); if (GroupSelected.State == Group.States.Expanded) visibleItems += GroupSelected.Items.Count; else visibleItems -= GroupSelected.Items.Count; }
                    else
                    {
                        if (GroupSelected.ItemSelected.OptionSelected == null) { if (GroupSelected.ItemSelected.Options.Count > 0) GroupSelected.ItemSelected.optionSelected = 0; }
                        else { GroupSelected.ItemSelected.optionSelected++; if (GroupSelected.ItemSelected.optionSelected >= GroupSelected.ItemSelected.Options.Count) GroupSelected.ItemSelected.optionSelected = 0; }
                    }
                }
            }
        }
        public void AddPrefix(string text) { prefixes.Add(new Title(text, Color.White)); }
        public void AddPrefix(string text, Color color) { prefixes.Add(new Title(text, color)); }
        public void AddSuffix(string text) { suffixes.Add(new Title(text, Color.White)); }
        public void AddSuffix(string text, Color color) { suffixes.Add(new Title(text, color)); }
        public void AddGroup(string name) { AddGroup(name, name); }
        public void AddGroup(string name, string title) { Groups.Add(name, new Group(title)); if (groupSelected == -1) groupSelected = 0; }
        public void AddItem(string group, string name) { AddItem(group, name, name); }
        public void AddItem(string group, string name, string title) { (Groups[group] as Group).Items.Add(name, new Group.Item(title)); }
        public void AddOption(string group, string item, string name) { AddOption(group, item, name, name, Color.White); }
        public void AddOption(string group, string item, string name, string title) { AddOption(group, item, name, title, Color.White); }
        public void AddOption(string group, string item, string name, Color color) { AddOption(group, item, name, name, color); }
        public void AddOption(string group, string item, string name, string title, Color color) { Group.Item i = ((Groups[group] as Group).Items[item] as Group.Item); i.Options.Add(name, new Group.Item.Option(title, color)); if (i.optionSelected == -1) i.optionSelected = 0; }
        public void ClearGroups() { groups.Clear(); }

        public void SelectOption(string group, string item, string option) { Group.Item i = ((Groups[group] as Group).Items[item] as Group.Item); if (i.Options.Contains(option)) for (int j = 0; j < i.Options.Count; j++) if ((i.Options.KeyFromIndex(j) as string) == option) { i.optionSelected = j; } }
        public string OptionSelected(string group, string item) { Group.Item i = ((Groups[group] as Group).Items[item] as Group.Item); if (i.OptionSelected != null) return i.OptionSelected.Name; else return null; }
        public Color? OptionSelectedColor(string group, string item) { Group.Item i = ((Groups[group] as Group).Items[item] as Group.Item); if (i.OptionSelected != null) return i.OptionSelected.Color; else return null; }

        public const float itemYSpace = 12.5f;
        public void Draw(Vector2 position, int width) { Draw(Screen.Batch, position, width, Font.Load("Consolas")); }
        public void Draw(Vector2 position, int width, SpriteFont font) { Draw(Screen.Batch, position, width, font); }
        public void Draw(Batch batch, Vector2 position, int width) { Draw(batch, position, width, Font.Load("Consolas")); }
        public void Draw(Batch batch, Vector2 position, int width, SpriteFont font)
        {
            float height = ((((prefixes.Count + suffixes.Count) * itemYSpace) + 9) + ((Groups.Count + visibleItems) * itemYSpace)); int j = 0;
            Rectangle rect = new Rectangle((int)Math.Floor(position.X), (int)Math.Floor(position.Y), width, (int)Math.Ceiling(height));
            Screen.FillRectangle(rect, (Color.Black * .75f));
            Vector2 fontScale = new Vector2(.125f);
            foreach (Title t in Prefixes) { Screen.DrawString(t.Text, font, new Vector2((position.X + (width / 2)), ((position.Y + 2) + (j * itemYSpace))), t.Color, new Textures.Origin(.5f, 0, true), fontScale); j++; }
            position.Y += 3;
            foreach (Group g in Groups.Values)
            {
                Screen.DrawString(g.Name, font, new Vector2((position.X + 4), (position.Y + (j * itemYSpace))), (((g == GroupSelected) && (g.ItemSelected == null)) ? GroupSelectedColor : GroupColor), fontScale);
                Screen.DrawString(("[" + ((g.State == Group.States.Expanded) ? "-" : "+") + "]"), font, new Vector2(((position.X + width) - 22), (position.Y + (j * itemYSpace))), (((g == GroupSelected) && (g.ItemSelected == null)) ? GroupSelectedColor : GroupColor), fontScale); j++;
                if (g.State == Group.States.Expanded)
                    foreach (Group.Item i in g.Items.Values)
                    {
                        if (i.OptionSelected != null) Screen.DrawString(("[" + i.OptionSelected.Name + "]"), font, new Vector2((((position.X + width) - (font.MeasureString(i.OptionSelected.Name).X * fontScale.X)) - 20), (position.Y + (j * itemYSpace))), ((i == g.ItemSelected) ? Color.Lerp(i.OptionSelected.Color, Color.White, .5f) : i.OptionSelected.Color), fontScale);
                        Screen.DrawString(i.Name, font, new Vector2((position.X + 12), (position.Y + (j * itemYSpace))), ((i == g.ItemSelected) ? ItemSelectedColor : ItemColor), fontScale); j++;
                    }
            }
            foreach (Title t in Suffixes) { Screen.DrawString(t.Text, font, new Vector2((position.X + (width / 2)), ((position.Y + 2) + (j * itemYSpace))), t.Color, new Textures.Origin(.5f, 0, true), fontScale); j++; }
        }

        public class Title
        {
            public string Text;
            public Color Color;

            public Title(string text, Color color) { Text = text; Color = color; }
        }

        public class Group
        {
            public string Name;

            internal OrderedDictionary items;
            public OrderedDictionary Items { get { return items; } }

            internal byte state = 1;
            public States State { get { return (States)state; } }
            public enum States { Expanded, Collapsed }

            public void Expand() { state = 0; }
            public void Collapse() { state = 1; }

            internal int itemSelected = -1;
            public Item ItemSelected { get { if (itemSelected == -1) return null; else { try { return (Items[itemSelected] as Item); } catch { itemSelected = -1; return null; } } } }

            public Group(string name) { items = new OrderedDictionary(); Name = name; }

            public class Item
            {
                public string Name;

                internal OrderedDictionary options;
                public OrderedDictionary Options { get { return options; } }

                internal int optionSelected = -1;
                public Option OptionSelected { get { if (optionSelected == -1) return null; else { try { return (Options[optionSelected] as Option); } catch { optionSelected = -1; return null; } } } }

                public Item(string name) { options = new OrderedDictionary(); Name = name; }

                public class Option
                {
                    public string Name;
                    public Color Color;

                    public Option(string name, Color color) { Name = name; Color = color; }
                }
            }
        }
    }
}