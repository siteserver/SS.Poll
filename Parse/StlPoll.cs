using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.Plugin;
using SS.Poll.Core;
using SS.Poll.Models;
using SS.Poll.Provider;

namespace SS.Poll.Parse
{
    internal class StlPoll
    {
        public const string ElementName = "stl:poll";

        public static object ApiSubmit(IRequest request, string id)
        {
            var pollId = Convert.ToInt32(id);
            var pollInfo = PollDao.GetPollInfo(pollId);
            if (pollInfo == null) return null;

            if (pollInfo.IsProfile)
            {
                var code = request.GetPostString("code");
                var cookie = request.GetCookie("ss-poll:" + id);
                if (string.IsNullOrEmpty(cookie) || !Utils.EqualsIgnoreCase(cookie, code))
                {
                    throw new Exception("提交失败，验证码不正确！");
                }
            }

            var itemIds = request.GetPostString("itemIds").Split(',').Select(idStr => Convert.ToInt32(idStr)).ToList();
            if (pollInfo.IsCheckbox)
            {
                if (pollInfo.CheckboxMin > 0 && itemIds.Count < pollInfo.CheckboxMin)
                {
                    throw new Exception($"提交失败，最少需要选择{pollInfo.CheckboxMin}项！");
                }
                if (pollInfo.CheckboxMax > 0 && itemIds.Count > pollInfo.CheckboxMax)
                {
                    throw new Exception($"提交失败，最多只能选择{pollInfo.CheckboxMax}项！");
                }
            }

            if (pollInfo.IsProfile)
            {
                var logInfo = new LogInfo
                {
                    SiteId = pollInfo.SiteId,
                    ChannelId = pollInfo.ChannelId,
                    ContentId = pollInfo.ContentId,
                    ItemIds = string.Join(",", itemIds),
                    AddDate = DateTime.Now
                };

                var attributes = request.GetPostObject<Dictionary<string, string>>("attributes");

                var fieldInfoList = FieldDao.GetFieldInfoList(pollInfo.SiteId, pollInfo.ChannelId, pollInfo.ContentId, false);
                foreach (var fieldInfo in fieldInfoList)
                {
                    string value;
                    attributes.TryGetValue(fieldInfo.AttributeName, out value);
                    logInfo.Set(fieldInfo.AttributeName, value);
                }

                LogDao.Insert(logInfo);
            }

            ItemDao.AddCount(pollInfo.SiteId, pollInfo.ChannelId, pollInfo.ContentId, itemIds);

            int totalCount;
            var itemInfoList = ItemDao.GetItemInfoList(pollInfo.SiteId, pollInfo.ChannelId, pollInfo.ContentId, out totalCount);
            var items = new List<object>();
            foreach (var itemInfo in itemInfoList)
            {
                var percentage = "0%";
                if (totalCount > 0)
                {
                    percentage = Convert.ToDouble(itemInfo.Count / (double)totalCount).ToString("0.0%");
                }
                items.Add(new
                {
                    itemInfo.Id,
                    itemInfo.SiteId,
                    itemInfo.ChannelId,
                    itemInfo.ContentId,
                    itemInfo.Title,
                    itemInfo.SubTitle,
                    itemInfo.ImageUrl,
                    itemInfo.Count,
                    Percentage = percentage
                });
            }

            return new
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        public static string Parse(IParseContext context)
        {
            if (context.SiteId <= 0 || context.ChannelId <= 0 || context.ContentId <= 0) return string.Empty;

            var pollInfo = PollDao.GetPollInfo(context.SiteId, context.ChannelId, context.ContentId);
            if (pollInfo == null) return string.Empty;

            var items = ItemDao.GetItemInfoList(context.SiteId, context.ChannelId, context.ContentId);
            if (items == null || items.Count == 0) return string.Empty;

            var itemBuilder = new StringBuilder();
            foreach (var itemInfo in items)
            {
                var imgHtml = pollInfo.IsImage
                    ? $@"<img src=""{itemInfo.ImageUrl}"" width=""163"" height=""163"" border=""0"" />"
                    : string.Empty;
                var inputHtml = pollInfo.IsCheckbox
                    ? $@"<input type=""checkbox"" name=""itemIds"" id=""item{itemInfo.Id}"" value=""{itemInfo.Id}"" v-model=""itemIds"" v-validate=""'required'"" :checked=""itemIds.indexOf('{itemInfo.Id}') > -1"" />"
                    : $@"<input type=""radio"" name=""itemIds"" id=""item{itemInfo.Id}"" value=""{itemInfo.Id}"" v-model=""itemIds"" v-validate=""'required'"" />";
                var titleHtml = itemInfo.Title;
                if (pollInfo.IsUrl)
                {
                    imgHtml = $@"<a href=""{itemInfo.LinkUrl}"" target=""_blank"">{imgHtml}</a>";
                    titleHtml = $@"<a href=""{itemInfo.LinkUrl}"" target=""_blank"">{titleHtml}</a>";
                }

                itemBuilder.Append($@"
<li>
    <label>
        {imgHtml}
        {inputHtml}
        <p>{titleHtml}</p>
        <p>{itemInfo.SubTitle}</p>
    </label>
</li>
");
            }

            string result;
            if (pollInfo.IsImage)
            {
                result = @"
<li v-for=""item in items"">
    <div class=""vote1_img"">
        <img v-bind:src=""item.imageUrl"" />
        <p>{{ item.title }}</p>
    </div>
    <div class=""vote1_num"">
        {{ item.count }}票
    </div>
    <div class=""vote1_plan"">
        <span v-bind:style=""{ width: item.percentage }""></span>
    </div>
    <div class=""vote1_percent"">
        {{ item.percentage }}
    </div>
</li>
";
            }
            else
            {
                result = @"
<li v-for=""item in items"">
    <div class=""vote1_text"">
        {{ item.title }}
    </div>
    <div class=""vote1_num"">
        {{ item.count }}票
    </div>
    <div class=""vote1_plan"">
        <span v-bind:style=""{ width: item.percentage }""></span>
    </div>
    <div class=""vote1_percent"">
        {{ item.percentage }}
    </div>
</li>
";
            }

            var pluginUrl = Context.PluginApi.GetPluginUrl(Main.PluginId, "assets");
            var pluginUrlCss = Context.PluginApi.GetPluginUrl(Main.PluginId, "css");
            var apiUrlSubmit = $"{Context.PluginApi.GetPluginApiUrl(Main.PluginId)}/{nameof(ApiSubmit)}/{pollInfo.Id}";
            var imgUrl = $"{Context.PluginApi.GetPluginApiUrl(Main.PluginId)}/code/{pollInfo.Id}";
            var submitHtml = string.Empty;
            if (pollInfo.IsProfile)
            {
                var fieldInfoList = FieldDao.GetFieldInfoList(context.SiteId, context.ChannelId, context.ContentId, true);
                var builder = new StringBuilder();

                foreach (var fieldInfo in fieldInfoList)
                {
                    if (fieldInfo.IsDisabled) continue;

                    var settings = new FieldSettings(fieldInfo.FieldSettings);
                    var fieldHtml = FieldTypeParser.Parse(fieldInfo, settings);

                    if (settings.IsValidate && settings.IsRequired)
                    {
                        builder.Append($@"
<li>
    <b><i>*</i>{fieldInfo.DisplayName}</b>
    {fieldHtml}
    <p>必填项</p>
</li>
");
                    }
                    else
                    {
                        builder.Append($@"
<li>
    <b>{fieldInfo.DisplayName}</b>
    {fieldHtml}
    <p></p>
</li>
");
                    }
                }

                submitHtml = @"
<ul>
";
                submitHtml += builder.ToString();
                submitHtml += @"
    <li>
        <b><i>*</i>验证码</b>
        <input v-model=""code"" name=""code"" v-validate=""'required'"" :class=""{'error': errors.has('code') }"" type=""text"" class=""w_input"" />
        <a href=""javascript:;"" v-on:click=""reload"">
            <img v-bind:src=""imgUrl"" />
        </a>
        <p>(不清楚?请点击刷新)</p>
    </li>
</ul>
";
            }

            return $@"
<link rel=""stylesheet"" type=""text/css"" href=""{pluginUrlCss}/base.css"" />
  <div id=""poll{pollInfo.Id}"">
    
    <template v-if=""isTimeout && timeToStart > new Date()"">
        <div class=""poll_wrap"">
          <div class=""poll_submit"">
            <h3 class=""error"" style=""display: block"">投票将于{pollInfo.TimeToStart:yyyy-MM-dd HH:mm}开始，请耐心等待！</h3>
          </div>
        </div>
    </template>

    <template v-if=""isTimeout && timeToEnd < new Date()"">
        <div class=""poll_wrap"">
          <div class=""poll_submit"">
            <h3 class=""error"" style=""display: block"">投票已结束，谢谢！</h3>
          </div>
        </div>
    </template>

    <template v-if=""(!isTimeout || (timeToStart < new Date() && timeToEnd > new Date())) && !items"">
        <div class=""poll_wrap"">
          <div class=""poll_list"">
            <ul>
                {itemBuilder}
            </ul>
          </div>
          <div class=""poll_submit"">
            <h3 class=""error"" style=""display: none"" v-show=""errors.has('itemIds')"">请选择投票项</h3>
            <h3 class=""error"" style=""display: none"" v-show=""errorMessage"" v-html=""errorMessage""></h3>
            {submitHtml}
            <a href=""javascript:;"" v-on:click=""submit"" class=""poll_btn"">提 交</a>
          </div>
        </div>
    </template>

    <template v-if=""(!isTimeout || (timeToStart < new Date() && timeToEnd > new Date())) && items"">
        <div class=""results_wrap"">
          <div class=""header_bg survey_bg""></div>
          <div class=""vote1_cont {(pollInfo.IsImage ? "" : "vote1_cont1")}"">
            <div class=""vote1_title"" v-show=""isResult"">
              总计：{{{{ totalCount }}}}票
            </div>
            <ul v-show=""isResult"">
              {result}
            </ul>
            <div class=""vote1_title"" v-show=""!isResult"">
              投票已提交，感谢参与！
            </div>
          </div>
        </div>
    </template>

  </div>

  <script src=""{pluginUrl}/js/vue-2.1.10.min.js"" type=""text/javascript""></script>
  <script src=""{pluginUrl}/js/vee-validate.js"" type=""text/javascript""></script>
  <script src=""{pluginUrl}/js/jquery.min.js"" type=""text/javascript""></script>
  <script type=""text/javascript"">
    Vue.use(VeeValidate);
    new Vue({{
      el: '#poll{pollInfo.Id}',
      data: {{
        isResult: {pollInfo.IsResult.ToString().ToLower()},
        isTimeout: {pollInfo.IsTimeout.ToString().ToLower()},
        timeToStart: new Date('{pollInfo.TimeToStart:yyyy-MM-dd HH:mm}'),
        timeToEnd: new Date('{pollInfo.TimeToEnd:yyyy-MM-dd HH:mm}'),
        itemIds: [],
        attributes: {{}},
        code: '',
        imgUrl: '{imgUrl}?' + new Date().getTime(),
        totalCount: 0,
        items: null,
        errorMessage: ''
      }},
      methods: {{
        reload: function (event) {{
          this.imgUrl = '{imgUrl}?' + new Date().getTime();
        }},
        submit: function (event) {{
          this.errorMessage = '';
          var $this = this;
          var data = {{
            itemIds: typeof(this.itemIds) === 'string' ? this.itemIds : this.itemIds.join(','),
            code: this.code,
            attributes: this.attributes
          }};
          
          this.$validator.validateAll().then(function (result) {{
            if (result) {{
                $.ajax({{
                    url : ""{apiUrlSubmit}"",
                    xhrFields: {{
                        withCredentials: true
                    }},
                    type: ""POST"",
                    data: JSON.stringify(data),
                    contentType: ""application/json; charset=utf-8"",
                    dataType: ""json"",
                    success: function(data)
                    {{
                        $this.totalCount = data.totalCount;
                        $this.items = data.items;
                    }},
                    error: function (err)
                    {{
                        var err = JSON.parse(err.responseText);
                        $this.errorMessage = err.message;
                    }}
                }});
            }}
          }});
        }}
      }}
    }});
  </script>
";
        }
    }
}
