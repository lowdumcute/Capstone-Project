using System;
using System.Linq;
using System.Collections.Generic;

namespace UniGLTF
{
    // Giả định glTFNode có: string name; int[] children;
    // Nếu namespace/type của bạn khác, nhớ chỉnh lại "glTFNode" cho đúng.
    public static class ForceGltfNodeUniqueName
    {
        public static void Process(List<glTFNode> nodes)
        {
            if (nodes == null) throw new ArgumentNullException(nameof(nodes));

            // Map: childIndex -> parentIndex (nếu có)
            var parentOf = BuildParentIndexMap(nodes);

            // Tập tên đã dùng để đảm bảo unique
            var used = new HashSet<string>(StringComparer.Ordinal);
            var counters = new Dictionary<string, int>(StringComparer.Ordinal);

            for (int i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var isLeaf = node.children == null || node.children.Length == 0;

                glTFNode parent = null;
                int pIndex;
                if (parentOf.TryGetValue(i, out pIndex) && pIndex >= 0 && pIndex < nodes.Count)
                {
                    parent = nodes[pIndex];
                }

                var parentName = parent != null && !string.IsNullOrWhiteSpace(parent.name)
                    ? parent.name
                    : (parent != null ? $"Parent{pIndex}" : null);

                var rawName = !string.IsNullOrWhiteSpace(node.name) ? node.name : $"Node{i}";

                // Nếu là lá và có parent -> ghép "parent-node"
                if (isLeaf && parent != null)
                {
                    rawName = $"{parentName}-{rawName}";
                }

                // Làm sạch tên cho Unity/Transform (tránh ký tự lạ, trim,…)
                var baseName = SanitizeName(rawName);

                // Đảm bảo duy nhất: name, name_1, name_2, ...
                node.name = MakeUnique(baseName, used, counters);
            }
        }

        private static Dictionary<int, int> BuildParentIndexMap(List<glTFNode> nodes)
        {
            var map = new Dictionary<int, int>();
            for (int p = 0; p < nodes.Count; p++)
            {
                var ch = nodes[p].children;
                if (ch == null) continue;
                for (int k = 0; k < ch.Length; k++)
                {
                    var c = ch[k];
                    if (!map.ContainsKey(c))
                    {
                        map[c] = p;
                    }
                }
            }
            return map;
        }

        private static string SanitizeName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "Node";
            name = name.Trim();

            // Thay các ký tự không an toàn bằng dấu gạch ngang
            // (Unity thường ổn, nhưng an toàn hơn: bỏ control chars)
            var chars = name.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                var ch = chars[i];
                if (char.IsControl(ch))
                {
                    chars[i] = '-';
                }
            }
            var cleaned = new string(chars);

            // Tránh tên trống sau khi làm sạch
            if (string.IsNullOrWhiteSpace(cleaned)) cleaned = "Node";
            return cleaned;
        }

        private static string MakeUnique(string baseName, HashSet<string> used, Dictionary<string, int> counters)
        {
            if (!used.Contains(baseName))
            {
                used.Add(baseName);
                return baseName;
            }

            int n;
            if (!counters.TryGetValue(baseName, out n))
            {
                n = 1;
            }

            string candidate;
            do
            {
                candidate = $"{baseName}_{n}";
                n++;
            } while (used.Contains(candidate));

            counters[baseName] = n;
            used.Add(candidate);
            return candidate;
        }
    }
}
