#!/usr/local/bin/python3
# -*- coding: utf-8 -*-
import os.path
import shutil
import time


def get_protocol_data():
    filename = "protocol.csv"
    raw_data = []
    try:
        inp = open(filename, mode='r', encoding='utf-8')
        lines = inp.readlines()
        for line in lines:
            fields = line.rstrip('\n').split(',')
            raw_data.append(fields)
        inp.close()
    except FileNotFoundError:
        print("File not found: " + filename)
    except Exception as e:
        print("Exception:%s\n%s" % (type(e), e))
    protocols = []
    cur_protocol = {"pid": "", "name": "", "desc": "", "req": [], "resp": [], "no_req": True}
    for line in raw_data:
        if line[0] == "pid":
            cur_protocol["pid"] = line[1]
        if line[0] == "name":
            cur_protocol["name"] = line[1]
        if line[0] == "desc":
            cur_protocol["desc"] = line[1]
        if line[0] == "req":
            cur_protocol["no_req"] = False
            if line[1] != "_PH":
                cur_protocol["req"].append({"type": line[1], "name": line[2], "desc": line[3]})
        if line[0] == "resp":
            cur_protocol["resp"].append({"type": line[1], "name": line[2], "desc": line[3]})
        if line[0] == "END":
            protocols.append(cur_protocol)
            cur_protocol = {"pid": "", "name": "", "desc": "", "req": [], "resp": [], "no_req": True}
    return protocols


def get_structure_data():
    filename = "structure.csv"
    raw_data = []
    try:
        inp = open(filename, mode='r', encoding='utf-8')
        lines = inp.readlines()
        for line in lines:
            fields = line.rstrip('\n').split(',')
            raw_data.append(fields)
        inp.close()
    except FileNotFoundError:
        print("File not found: " + filename)
    except Exception as e:
        print("Exception:%s\n%s" % (type(e), e))
    structures = []
    cur_item = {"name": "", "desc": "", "fields": []}
    for line in raw_data:
        if line[0] == "name":
            cur_item["name"] = line[1]
        elif line[0] == "desc":
            cur_item["desc"] = line[1]
        elif line[0] == "END":
            structures.append(cur_item)
            cur_item = {"name": "", "desc": "", "fields": []}
        else:
            cur_item["fields"].append({"type": line[0], "name": line[1], "desc": line[2]})
    return structures


def write_client_protool(protocols):
    filename = "SFMsgClass.protocol.cs"
    file = open(filename, mode="wt", encoding="utf-8")
    template = open("clientProtocolTemplate.txt", mode="r", encoding="utf-8")
    res = template.readlines()
    template.close()
    for item in protocols:
        # {"pid", "name", "desc", "req": [], "resp": [], "no_req": True}
        pid = item["pid"]
        name = item["name"]
        desc = item["desc"]
        req = item["req"]
        resp = item["resp"]
        no_req = item["no_req"]
        res += "\n" \
               "    #region %s-%s\n" % (pid, desc)
        # request
        if not no_req:
            res += "    /// <summary>\n"
            res += "    /// [Req]%s\n" % desc
            res += "    /// </summary>\n"
            res += "    [Serializable]\n"
            res += "    public class SFRequestMsg%s : SFBaseRequestMessage\n" % name
            res += "    {\n" \
                   "        public const string pName = \"socket_%s\";\n" % pid
            res += "        public SFRequestMsg%s()\n" \
                   "        {\n" \
                   "            pid = %s;\n" \
                   "        }\n" % (name, pid)
            for req_item in req:
                if req_item["desc"] != "":
                    res += "\n" \
                           "        /// <summary>\n" \
                           "        /// %s\n" \
                           "        /// </summary>\n" % req_item["desc"]
                res += "        public %s %s;\n" % (req_item["type"], req_item["name"])
            # end for
            pass
            res += "    };\n\n"
        # end if
        pass

        # response
        prefix = "[Resp]"
        if no_req:
            prefix = "[Resp][Notify]"
        res += "    /// <summary>\n" \
               "    /// %s%s\n" \
               "    /// </summary>\n" % (prefix, desc)
        res += "    [Serializable]\n" \
               "    public class SFResponseMsg%s : SFBaseResponseMessage\n" \
               "    {\n" % name
        res += "        public const string pName = \"socket_%s\";\n" % pid
        res += "        public SFResponseMsg%s()\n" \
               "        {\n" \
               "            pid = %s;\n" \
               "        }\n" % (name, pid)
        for resp_item in resp:
            if resp_item["desc"] != "":
                res += "\n" \
                       "        /// <summary>\n" \
                       "        /// %s\n" \
                       "        /// </summary>\n" % resp_item["desc"]
            # end if
            pass
            res += "        public %s %s;\n" % (resp_item["type"], resp_item["name"])
        # end for
        pass
        res += "    };\n" \
               "    #endregion\n"
    # end for
    pass
    res += "}\n" \
           "// Last Update: %s\n" % time.strftime("%Y/%m/%d")
    file.writelines(res)
    file.close()
    print("Write client protool completed.")


def write_client_structure(structures):
    filename = "SFMsgData.protocol.cs"
    file = open(filename, mode="wt", encoding="utf-8")
    res = "using System;\n" \
          "using System.Collections;\n" \
          "using System.Collections.Generic;\n" \
          "using UnityEngine;\n" \
          "\n" \
          "namespace SF\n" \
          "{\n"
    for item in structures:
        # {"name", "desc", "fields":[]}
        name = item["name"]
        desc = item["desc"]
        fields = item["fields"]
        res += "    /// <summary>\n" \
               "    /// %s\n" \
               "    /// </summary>\n" % desc
        res += "    [Serializable]\n" \
               "    public struct SFMsgData%s\n" \
               "    {\n" % name
        for field in fields:
            if field["desc"] != "":
                res += "\n" \
                       "        /// <summary>\n" \
                       "        /// %s\n" \
                       "        /// <summary>\n" % field["desc"]
            # end if
            pass
            res += "        public %s %s;\n" % (field["type"], field["name"])
        # end for
        pass
        res += "    };\n\n"
    # end for
    pass
    res += "}\n"
    file.writelines(res)
    file.close()
    print("Write client structure completed.")


def write_server(protocols, structures):
    filename = "SFProtocolMessage.protocol.js"
    file = open(filename, mode="wt", encoding="utf-8")
    res = "/**\n" \
          " * Last Update: %s\n" \
          " */\n" \
          "\"use strict\";\n" \
          "\n" % time.strftime("%Y/%m/%d")
    # structures
    for item in structures:
        # {"name", "desc", "fields":[]}
        name = item["name"]
        desc = item["desc"]
        fields = item["fields"]
        res += "/**\n" \
               " * %s\n" % desc
        str_fields = ""
        for field in fields:
            str_type = field["type"]
            if str_type == "int" or str_type == "float":
                str_type = "number"
            if str_type.startswith("List<"):
                str_type = str_type.replace("List<", "list<")
            str_desc = ""
            if field["desc"] != "":
                str_desc = " - %s" % field["desc"]
            res += " * @param {%s} %s%s\n" % (str_type, field["name"], str_desc)
            str_default_value = "[]"
            if str_type == "string":
                str_default_value = "\"\""
            if str_type == "number":
                str_default_value = "0"
            str_fields += "        this.%s = %s;\n" % (field["name"], str_default_value)
        # end for
        pass
        res += " */\n" \
               "class SFMsgData%s {\n" \
               "    constructor() {\n" \
               "%s" \
               "    }\n" \
               "}\n" \
               "exports.SFMsgData%s = SFMsgData%s;\n" \
               "\n" % (name, str_fields, name, name)
    # end for
    pass

    # protocols
    res += "\nconst requests = {};\n\n"
    for item in protocols:
        # {"pid", "name", "desc", "req": [], "resp": [], "no_req": True}
        pid = item["pid"]
        name = item["name"]
        desc = item["desc"]
        req = item["req"]
        resp = item["resp"]
        no_req = item["no_req"]
        # request
        if not no_req:
            res += "/**\n" \
                   " * [REQ]%s\n" % desc
            str_fields = ""
            for req_item in req:
                str_desc = req_item["desc"]
                str_type = req_item["type"]
                if str_type == "int" or str_type == "float":
                    str_type = "number"
                if str_type.startswith("List<"):
                    str_type = str_type.replace("List<", "list<")
                if str_desc != "":
                    str_desc = " - %s" % req_item["desc"]
                res += " * @param {%s} %s%s\n" % (str_type, req_item["name"], str_desc)
                str_fields += "        this.%s = obj['%s'];\n" % (req_item["name"], req_item["name"])
            # end for
            pass
            res += " */\n"
            res += "class SFRequest%s {\n" \
                   "    constructor(data) {\n" \
                   "        let obj = null;\n" \
                   "        try {\n" \
                   "            obj = JSON.parse(data);\n" \
                   "        }\n" \
                   "        catch (e) {\n" \
                   "            obj = {};\n" \
                   "        }\n" \
                   "        this.pid = %s;\n" \
                   "        this.uid = obj['uid'];\n" % (name, pid)
            res += str_fields
            res += "    }\n" \
                   "}\n" \
                   "exports.SFRequest%s = SFRequest%s;\n" \
                   "requests['socket_%s'] = SFRequest%s;\n" \
                   "\n" % (name, name, pid, name)
        # end if
        pass
        # response
        res += "/**\n" \
               " * [RESP]%s\n" % desc
        str_fields = ""
        for resp_item in resp:
            str_desc = resp_item["desc"]
            str_type = resp_item["type"]
            if str_type == "int" or str_type == "float":
                str_type = "number"
            if str_type.startswith("List<"):
                str_type = str_type.replace("List<", "list<")
            if str_desc != "":
                str_desc = " - %s" % resp_item["desc"]
            res += " * @param {%s} %s%s\n" % (str_type, resp_item["name"], str_desc)
            str_default_value = "[]"
            if str_type == "string":
                str_default_value = "\"\""
            if str_type == "number":
                str_default_value = "0"
            str_fields += "        this.%s = %s;\n" % (resp_item["name"], str_default_value)
        # end for
        pass
        res += " */\n" \
               "class SFResponse%s {\n" \
               "    constructor() {\n" \
               "        this.pid = %s;\n" \
               "        this.retCode = 0;\n" \
               "%s" \
               "    }\n" \
               "}\n" \
               "exports.SFResponse%s = SFResponse%s;\n" \
               "\n" % (name, pid, str_fields, name, name)
    # end for
    pass
    res += "exports.requests = requests;\n"
    file.writelines(res)
    file.close()
    print("Write server completed.")


def main():
    structures = get_structure_data()
    print("%d structures:" % len(structures))
    idx = 0
    for item in structures:
        idx += 1
        print(" - %02d. %s" % (idx, item["desc"]))
    protocols = get_protocol_data()
    print("%d protocols:" % len(protocols))
    idx = 0
    for item in protocols:
        idx += 1
        print(" - %02d. %s" % (idx, item["desc"]))

    write_client_protool(protocols)
    write_client_structure(structures)
    write_server(protocols, structures)

    # copy
    source_filename = "./SFMsgClass.protocol.cs"
    target_filename = "./../../Assets/Scripts/Network/SFMsgClass.cs"
    if os.path.exists(source_filename) and os.path.exists(target_filename):
        shutil.copyfile(source_filename, target_filename)
        print("Copied SFMsgClass.cs")
    else:
        print("Did not copy SFMsgClass.cs")

    source_filename = "./SFMsgData.protocol.cs"
    target_filename = "./../../Assets/Scripts/Network/SFMsgData.cs"
    if os.path.exists(source_filename) and os.path.exists(target_filename):
        shutil.copyfile(source_filename, target_filename)
        print("Copied SFMsgData.cs")
    else:
        print("Did not copy SFMsgData.cs")

    source_filename = "./SFProtocolMessage.protocol.js"
    target_filename = "./../../server/app/gameServer/SFProtocolMessage.js"
    if os.path.exists(source_filename) and os.path.exists(target_filename):
        shutil.copyfile(source_filename, target_filename)
        print("Copied SFProtocolMessage.js")
    else:
        print("Did not copy SFProtocolMessage.js")

    # TODO
    for item in protocols:
      pid = item["pid"]
      name = item["name"]
      res = ""
      res += "                else if (pid == %s)\n" \
             "                {\n" \
             "                    obj = JsonUtility.FromJson<SFResponseMsg%s>(data);\n" \
             "                }" % (pid, name)
      print(res)
    # end for
    pass


if __name__ == "__main__":
    main()