namespace RiverLine.Api.Services.Email;

 
public sealed record EmailTemplate(string Subject, string Body);
 
public static class EmailTemplates
{
    private static readonly Dictionary<string, EmailTemplate> Templates = new(
        StringComparer.OrdinalIgnoreCase)
    {
        ["offer-received"] = new(
            Subject: "وصلك عرض جديد على شحنتك",
            Body: """
                <p>أهلاً {{recipientName}},</p>
                <p>
                  وصلك عرض جديد على شحنة <strong>{{cargoType}}</strong>
                  من {{origin}} إلى {{destination}}.
                </p>
                <p class="figure">{{price}} جنيه</p>
                <p>تاريخ الشحن المقترح: {{pickupDate}}</p>
                <p><a class="btn" href="{{actionUrl}}">شوف العرض</a></p>
                """),
 
        ["offer-accepted"] = new(
            Subject: "عرضك اتقبل",
            Body: """
                <p>أهلاً {{recipientName}},</p>
                <p>
                  صاحب الشحنة قبل عرضك بـ <strong>{{price}} جنيه</strong>
                  للشحنة من {{origin}} إلى {{destination}}.
                </p>
                <p>تاريخ الشحن: {{pickupDate}}</p>
                <p><a class="btn" href="{{actionUrl}}">تفاصيل الشحنة</a></p>
                """),
 
        ["shipment-cancelled"] = new(
            Subject: "اتلغت شحنة",
            Body: """
                <p>أهلاً {{recipientName}},</p>
                <p>
                  الشحنة من {{origin}} إلى {{destination}} اتلغت.
                </p>
                <p>السبب: {{reason}}</p>
                <p><a class="btn" href="{{actionUrl}}">شوف التفاصيل</a></p>
                """),
    };
 
    public static EmailTemplate? Get(string key) => Templates.GetValueOrDefault(key);
 
    // ⚠️ dir="rtl" و lang="ar" لازم يكونوا على <html> نفسه.
    // عملاء الإيميل (خصوصاً Outlook) بيتجاهلوا الـ CSS كتير،
    // فالـ attributes دي هي اللي بتظبط الاتجاه فعلاً.
    public const string Layout = """
        <!DOCTYPE html>
        <html dir="rtl" lang="ar">
          <head><meta charset="utf-8" /></head>
          <body style="margin:0;background:#f8f9eb;
                       font-family:'Segoe UI',Tahoma,Arial,sans-serif;">
            <table role="presentation" width="100%" cellpadding="0" cellspacing="0">
              <tr><td align="center" style="padding:32px 16px;">
                <table role="presentation" width="100%"
                       style="max-width:560px;background:#ffffff;border-radius:14px;
                              border:1px solid #e6e6dd;">
                  <tr><td style="padding:28px 28px 8px;">
                    <span style="font-size:13px;font-weight:700;letter-spacing:.04em;
                                 color:#3a6410;">RIVERLINE</span>
                  </td></tr>
                  <tr><td style="padding:0 28px 28px;font-size:15px;line-height:26px;
                                 color:#1a1a17;">
                    {{content}}
                  </td></tr>
                </table>
                <p style="margin:16px 0 0;font-size:12px;color:#6b6b60;">
                  بتوصلك الرسالة دي لأن عندك حساب على RiverLine.
                </p>
              </td></tr>
            </table>
          </body>
        </html>
        """;
}