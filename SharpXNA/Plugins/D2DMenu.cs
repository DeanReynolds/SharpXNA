using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using Microsoft.Xna.Framework.Input;
using Keyboard = SharpXNA.Input.Keyboard;
using System;

namespace SharpXNA.Plugins
{
    public class D2DMenu
    {
        public Color GroupColor = Color.Red, GroupSelectedColor = Color.Yellow, ItemColor = Color.White, ItemSelectedColor = Color.Yellow;

        internal int Index = -1;
        public GroupCollection Groups;

        internal int VisibleItems = 1;
        internal OrderedDictionary Prefixes, Suffixes;

        public void AddPrefix(string text) { if (Prefixes.Contains(text)) throw new Exception($"Prefix with name '{text}' already exists!"); Prefixes.Add(text, new Title(text)); VisibleItems++; }
        public void AddPrefix(string text, Color color) { if (Prefixes.Contains(text)) throw new Exception($"Prefix with name '{text}' already exists!"); Prefixes.Add(text, new Title(text, color)); VisibleItems++; }
        public void RemovePrefix(string text) { if (!Prefixes.Contains(text)) return; Prefixes.Remove(text); VisibleItems--; }
        public void AddSuffix(string text) { if (Suffixes.Contains(text)) throw new Exception($"Suffix with name '{text}' already exists!"); Suffixes.Add(text, new Title(text)); VisibleItems++; }
        public void AddSuffix(string text, Color color) { if (Suffixes.Contains(text)) throw new Exception($"Suffix with name '{text}' already exists!"); Suffixes.Add(text, new Title(text, color)); VisibleItems++; }
        public void RemoveSuffix(string text) { if (!Suffixes.Contains(text)) return; Suffixes.Remove(text); VisibleItems--; }

        public D2DMenu(string name) { Load(name, Color.White); }
        public D2DMenu(string name, Color color) { Load(name, color); }

        public void Load(string name, Color color)
        {
            Prefixes = new OrderedDictionary() { { name, new Title(name) } };
            Suffixes = new OrderedDictionary();
            Groups = new GroupCollection();
        }

        public void AcceptInput()
        {
            if (Keyboard.Pressed(Keys.Up))
            {
                if (Index >= 0)
                {
                    if (!GoUp(Groups[Index]))
                    {
                        Index--; if (Index == -1) Index = (Groups.Count - 1);
                        if (Groups[Index].Open) Groups[Index].Index = ((Groups[Index].Groups.Count + Groups[Index].Items.Count) - 1);
                    }
                }
            }
            if (Keyboard.Pressed(Keys.Down))
            {
                if (Index >= 0)
                {
                    if (!GoDown(Groups[Index])) Index++;
                    if (Index >= Groups.Count) Index = 0;
                }
            }
            if (Keyboard.Pressed(Keys.Left)) { if (Index >= 0) GoLeft(Groups[Index]); }
            if (Keyboard.Pressed(Keys.Right)) { if (Index >= 0) GoRight(Groups[Index]); }
        }
        internal bool GoUp(Group group)
        {
            if (!group.Open) return false;
            if ((group.Index >= 0) && (group.Index < group.Groups.Count) && group.Groups[group.Index].Open) if (GoUp(group.Groups[group.Index])) return true;
            group.Index--;
            if (group.Index == -1) return true;
            else if (group.Index < -1) group.Index = -1;
            if ((group.Index >= 0) && (group.Index < group.Groups.Count) && group.Groups[group.Index].Open) group.Groups[group.Index].Index = ((group.Groups[group.Index].Groups.Count + group.Groups[group.Index].Items.Count) - 1);
            return (group.Index >= 0);
        }
        internal bool GoDown(Group group)
        {
            if (!group.Open) return false;
            if ((group.Index >= 0) && (group.Index < group.Groups.Count) && group.Groups[group.Index].Open) if (GoDown(group.Groups[group.Index])) return true;
            group.Index++;
            if (group.Index >= (group.Groups.Count + group.Items.Count)) { group.Index = -1; return false; }
            if ((group.Index >= 0) && (group.Index < group.Groups.Count)) group.Groups[group.Index].Index = -1;
            return true;
        }
        internal bool GoLeft(Group group)
        {
            if (group.Open)
            {
                if (group.Index >= 0)
                {
                    if (group.Index >= group.Groups.Count)
                    {
                        if (group.Index < (group.Groups.Count + group.Items.Count))
                        {
                            var item = group.Items[group.Index - group.Groups.Count];
                            item.Index--;
                            if (item.Index == -1) { item.Index = (item.Options.Count - 1); return (item.Index != 0); }
                            return true;
                        }
                    }
                    else GoLeft(group.Groups[group.Index]);
                    return false;
                }
                CloseGroup(group);
                return true;
            }
            OpenGroup(group);
            return true;
        }
        internal bool GoRight(Group group)
        {
            if (group.Open)
            {
                if (group.Index >= 0)
                {
                    if (group.Index >= group.Groups.Count)
                    {
                        if (group.Index < (group.Groups.Count + group.Items.Count))
                        {
                            var item = group.Items[group.Index - group.Groups.Count];
                            item.Index++;
                            if (item.Index >= item.Options.Count) { item.Index = 0; return (item.Index != item.Options.Count); }
                            return true;
                        }
                    }
                    else GoRight(group.Groups[group.Index]);
                    return false;
                }
                CloseGroup(group);
                return true;
            }
            OpenGroup(group);
            return true;
        }

        internal void OpenGroup(Group group)
        {
            if (group.Open) return;
            group.Open = true;
            VisibleItems += GetVisibleItems(group);
        }
        internal void CloseGroup(Group group)
        {
            if (!group.Open) return;
            VisibleItems -= GetVisibleItems(group);
            group.Open = false;
        }
        internal int GetVisibleItems(Group group)
        {
            if (!group.Open) return 0;
            var visibleItems = (group.Groups.Count + group.Items.Count);
            for (var i = 0; i < group.Groups.Count; i++) visibleItems += GetVisibleItems(group.Groups[i]);
            return visibleItems;
        }

        public void Draw(Vector2 position, SpriteFont font, int width, float textScale) { Draw(Screen.Batch, position, font, width, textScale); }
        public void Draw(Batch batch, Vector2 position, SpriteFont font, int width, float textScale)
        {
            batch.FillRectangle(new Rectangle((int)(Math.Floor(position.X) - 4), (int)(Math.Floor(position.Y) - 4), (width + 8), (int)(Math.Ceiling(VisibleItems * (font.MeasureString("Z").Y * textScale)) + 8)), (Color.Black * .75f));
            for (var i = 0; i < Prefixes.Count; i++)
            {
                var fix = (Title)Prefixes[i];
                batch.DrawString(fix.Text, font, new Vector2((position.X + (width / 2)), position.Y), fix.Color, new Origin(.5f, 0, true), new Vector2(textScale));
                position.Y += (font.MeasureString(fix.Text).Y * textScale);
            }
            byte indent = 0;
            for (var i = 0; i < Groups.Count; i++) DrawGroup(Groups[i], null, batch, ref position, font, ref indent, width, textScale);
            for (var i = 0; i < Suffixes.Count; i++)
            {
                var fix = (Title)Suffixes[i];
                batch.DrawString(fix.Text, font, new Vector2((position.X + (width / 2)), position.Y), fix.Color, new Origin(.5f, 0, true), new Vector2(textScale));
                position.Y += (font.MeasureString(fix.Text).Y * textScale);
            }
        }
        internal void DrawGroup(Group group, Group parent, Batch batch, ref Vector2 position, SpriteFont font, ref byte indent, int width, float textScale)
        {
            batch.DrawString(group.Name, font, new Vector2((position.X + (10 * indent)), position.Y), (((((parent == null) && (group == Groups[Index])) || ((parent != null) &&
                (parent.Index >= 0) && (parent.Groups.Count > parent.Index) && (group == parent.Groups[parent.Index]))) && (group.Index == -1)) ? GroupSelectedColor : GroupColor), new Vector2(textScale));
            batch.DrawString($"[{(group.Open ? '-' : '+')}]", font, new Vector2(((position.X + width) - ((font.MeasureString($"[{(group.Open ? '-' : '+')}]").X * textScale) + (10 * indent))), position.Y), (((((parent == null) && (group == Groups[Index])) || ((parent != null) &&
                (parent.Index >= 0) && (parent.Groups.Count > parent.Index) && (group == parent.Groups[parent.Index]))) && (group.Index == -1)) ? GroupSelectedColor : GroupColor), new Vector2(textScale));
            position.Y += (font.MeasureString(group.Name).Y * textScale);
            if (group.Open)
            {
                var oldIndent = indent;
                if (group.Groups.Count > 0)
                {
                    indent++;
                    var x2 = position.X;
                    for (var i = 0; i < group.Groups.Count; i++)
                    {
                        DrawGroup(group.Groups[i], group, batch, ref position, font, ref indent, width, textScale);
                        position.X = x2;
                    }
                    indent--;
                }
                if (group.Items.Count > 0)
                {
                    indent++;
                    for (var i = 0; i < group.Items.Count; i++)
                    {
                        batch.DrawString(group.Items[i].Name, font, new Vector2((position.X + (10 * indent)), position.Y),
                            (((group.Index >= group.Groups.Count) && (i == (group.Index - group.Groups.Count))) ? ItemSelectedColor : ItemColor), new Vector2(textScale));
                        batch.DrawString(group.Items[i].OptionName, font, new Vector2(((position.X + width) - ((font.MeasureString(group.Items[i].OptionName).X * textScale) + (10 * indent))), position.Y),
                            (((group.Index >= group.Groups.Count) && (i == (group.Index - group.Groups.Count))) ? Color.Lerp(ItemSelectedColor, (group.Items[i].OptionColor ?? Color.White), .75f)
                            : (group.Items[i].OptionColor ?? Color.White)), new Vector2(textScale));
                        position.Y += (font.MeasureString(group.Items[i].Name).Y * textScale);
                    }
                    indent--;
                }
                indent = oldIndent;
            }
        }

        public void AddGroup(string name) { if (Groups.Contains(name)) throw new Exception($"Group with name '{name}' already exists!"); Groups.Add(name, new Group(name)); if (Index == -1) Index = 0; VisibleItems++; }

        public void RemoveGroup(string name) { if (Groups.Contains(name)) { Groups.Remove(name); VisibleItems--; } while (Index > Groups.Count) Index--; }
        public void RemoveGroupAt(int index) { if (Groups.Count > index) { Groups.RemoveAt(index); VisibleItems--; } while (Index > Groups.Count) Index--; }

        public string GroupName
        {
            get
            {
                if ((Index >= 0) && (Groups.Count > Index)) return Groups[Index].Name;
                return null;
            }
        }

        public class Title
        {
            public string Text;
            public Color Color = Color.White;

            public Title(string text) { Text = text; }
            public Title(string text, Color color) { Text = text; Color = color; }
        }

        public class GroupCollection
        {
            private readonly OrderedDictionary _groups;

            public GroupCollection() { _groups = new OrderedDictionary(); }
            public GroupCollection(int capacity) { _groups = new OrderedDictionary(capacity); }

            public Group this[int index]
            {
                get
                {
                    if (_groups.Count > index) return (Group)_groups[index];
                    return null;
                }
            }
            public Group this[string name]
            {
                get
                {
                    if (_groups.Contains(name)) return (Group)_groups[name];
                    return null;
                }
            }

            public bool Contains(string name) => _groups.Contains(name);
            public void Add(string name, Group group) { _groups.Add(name, group); }
            public void Remove(string name) { _groups.Remove(name); }
            public void RemoveAt(int index) { _groups.RemoveAt(index); }

            public int Count => _groups.Count;
        }

        public class Group
        {
            public string Name;
            public bool Open;

            public Group(string name) { Groups = new GroupCollection(); Items = new ItemCollection(); Name = name; }

            internal int Index = -1;
            public GroupCollection Groups;
            public ItemCollection Items;

            public void AddGroup(string name) { if (!Groups.Contains(name)) Groups.Add(name, new Group(name)); }
            public void AddItem(string name) { if (!Items.Contains(name)) Items.Add(name, new Item(name)); }

            public void RemoveGroup(string name) { if (Groups.Contains(name)) Groups.Remove(name); while (Index > ((Groups.Count + Items.Count))) Index--; }
            public void RemoveGroupAt(int index) { if (Groups.Count > index) Groups.RemoveAt(index); while (Index > ((Groups.Count + Items.Count))) Index--; }
            public void RemoveItem(string name) { if (Items.Contains(name)) Items.Remove(name); while (Index > ((Groups.Count + Items.Count))) Index--; }
            public void RemoveItemAt(int index) { if (Items.Count > index) Items.RemoveAt(index); while (Index > ((Groups.Count + Items.Count))) Index--; }

            public string GroupName
            {
                get
                {
                    if ((Index >= 0) && (Groups.Count > Index)) return Groups[Index].Name;
                    return null;
                }
            }
            public string ItemName
            {
                get
                {
                    if ((Index >= 0) && ((Groups.Count + Items.Count) > Index)) return Items[Index - Groups.Count].Name;
                    return null;
                }
            }

            public class ItemCollection
            {
                private readonly OrderedDictionary _items;

                public ItemCollection() { _items = new OrderedDictionary(); }
                public ItemCollection(int capacity) { _items = new OrderedDictionary(capacity); }

                public Item this[int index]
                {
                    get
                    {
                        if (_items.Count > index) return (Item)_items[index];
                        return null;
                    }
                }
                public Item this[string name]
                {
                    get
                    {
                        if (_items.Contains(name)) return (Item)_items[name];
                        return null;
                    }
                }

                public bool Contains(string name) => _items.Contains(name);
                public void Add(string name, Item item) { _items.Add(name, item); }
                public void Remove(string name) { _items.Remove(name); }
                public void RemoveAt(int index) { _items.RemoveAt(index); }

                public int Count => _items.Count;
            }

            public class Item
            {
                public string Name;

                public Item(string name) { Options = new OptionCollection(); Name = name; }

                internal int Index = -1;
                public OptionCollection Options;

                public void AddOption(string name) { if (!Options.Contains(name)) Options.Add(name, new Option(name)); if (Index == -1) Index = 0; }
                public void AddOption(string name, Color color) { if (!Options.Contains(name)) Options.Add(name, new Option(name, color)); if (Index == -1) Index = 0; }

                public void RemoveOption(string name) { if (Options.Contains(name)) Options.Remove(name); while (Index > Options.Count) Index--; }
                public void RemoveOptionAt(int index) { if (Options.Count > index) Options.RemoveAt(index); while (Index > Options.Count) Index--; }
                public void ClearOptions() { Options.Clear(); }

                public string OptionName
                {
                    get
                    {
                        if ((Index >= 0) && (Options.Count > Index)) return Options[Index].Name;
                        return null;
                    }
                }
                public Color? OptionColor
                {
                    get
                    {
                        if ((Index >= 0) && (Options.Count > Index)) return Options[Index].Color;
                        return null;
                    }
                }

                public class OptionCollection
                {
                    private readonly OrderedDictionary _options;

                    public OptionCollection() { _options = new OrderedDictionary(); }
                    public OptionCollection(int capacity) { _options = new OrderedDictionary(capacity); }

                    public Option this[int index]
                    {
                        get
                        {
                            if (_options.Count > index) return (Option)_options[index];
                            return null;
                        }
                    }
                    public Option this[string name]
                    {
                        get
                        {
                            if (_options.Contains(name)) return (Option)_options[name];
                            return null;
                        }
                    }

                    public bool Contains(string name) => _options.Contains(name);
                    public void Add(string name, Option item) { _options.Add(name, item); }
                    public void Remove(string name) { _options.Remove(name); }
                    public void RemoveAt(int index) { _options.RemoveAt(index); }
                    public void Clear() { _options.Clear(); }

                    public int Count => _options.Count;
                }

                public class Option
                {
                    public readonly string Name;
                    public readonly Color Color = Color.White;

                    public Option(string name) { Name = name; }
                    public Option(string name, Color color) { Name = name; Color = color; }
                }
            }
        }
    }
}