using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QMSApplication.Models;

namespace QMSApplication.Filters
{
    public class MandatoryNotificationFilter : IAsyncPageFilter
    {
        private readonly QMSPortalDbContext _db;

        public MandatoryNotificationFilter(QMSPortalDbContext db)
        {
            _db = db;
        }

        public Task OnPageHandlerSelectionAsync(
            PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(
            PageHandlerExecutingContext context,
            PageHandlerExecutionDelegate next)
        {
            var pagePath = context.HttpContext.Request.Path.Value;

            // Allow the Notifications page so the user can open
            // and acknowledge the mandatory notification.
            if (pagePath != null &&
                pagePath.StartsWith("/Notifications",
                    StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            var mandatoryPending = await _db.Notifications
                .AsNoTracking()
                .AnyAsync(x =>
                    x.IsPublished &&
                    x.IsMandatory &&
                    !x.IsAcknowledged);

            if (mandatoryPending)
            {
                context.Result = new RedirectToPageResult("/Notifications");
                return;
            }

            await next();
        }
    }
}