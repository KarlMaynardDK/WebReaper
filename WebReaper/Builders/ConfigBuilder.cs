using System.Collections.Immutable;
using WebReaper.ConfigStorage;
using WebReaper.Domain;
using WebReaper.Domain.Parsing;
using WebReaper.Domain.Selectors;
using WebReaper.PageActions;

namespace WebReaper.Builders;

public class ConfigBuilder
{
    private readonly List<LinkPathSelector> _linkPathSelectors = new();

    private IEnumerable<string> _blockedUrls = Enumerable.Empty<string>();
    
    private string _startUrl;

    private Schema? _schema;
    
    private PageType _startPageType;

    private List<PageAction>? _pageActions = null;
    
    private int _pageCrawlLimit = Int32.MaxValue;

    public ConfigBuilder Get(string startUrl)
    {
        _startUrl = startUrl;
        _startPageType = PageType.Static;

        return this;
    }

    public ConfigBuilder GetWithBrowser(
        string startUrl,
        List<PageAction>? pageActions = null)
    {
        _startUrl = startUrl;

        _startPageType = PageType.Dynamic;
        _pageActions = pageActions;

        return this;
    }

    public ConfigBuilder Follow(string linkSelector)
    {
        _linkPathSelectors.Add(new LinkPathSelector(linkSelector, null, PageType.Static));
        return this;
    }

    public ConfigBuilder FollowWithBrowser(
        string linkSelector,
        List<PageAction>? pageActions = null)
    {
        _linkPathSelectors.Add(new LinkPathSelector(linkSelector, null, PageType.Dynamic, pageActions));
        return this;
    }
    
    public ConfigBuilder IgnoreUrls(IEnumerable<string> urls)
    {
        _blockedUrls = urls;
        return this;
    }
    
    public ConfigBuilder WithPageCrawlLimit(int limit)
    {
        _pageCrawlLimit = limit;
        return this;
    }

    public ConfigBuilder Paginate(
        string linkSelector,
        string paginationSelector)
    {
        _linkPathSelectors.Add(new LinkPathSelector(linkSelector, paginationSelector, PageType.Static));
        return this;
    }

    public ConfigBuilder PaginateWithBrowser(
        string linkSelector,
        string paginationSelector,
        List<PageAction>? pageActions = null)
    {
        _linkPathSelectors.Add(new LinkPathSelector(linkSelector, paginationSelector, PageType.Dynamic, pageActions));
        return this;
    }

    public ConfigBuilder WithScheme(Schema schema)
    {
        _schema = schema;
        return this;
    }

    public ScraperConfig Build()
    {
        if (_startUrl is null) throw new InvalidOperationException($"Start Url is missing. You must call the {nameof(Get)} or {nameof(GetWithBrowser)} method");
        if (_schema is null) throw new InvalidOperationException($"You must call the {nameof(WithScheme)} method to set the parsing scheme");

        return new ScraperConfig(
            _schema,
            ImmutableQueue.Create(_linkPathSelectors.ToArray()),
            _startUrl,
            _blockedUrls,
            _pageCrawlLimit,
            _startPageType,
            _pageActions);
    }
}