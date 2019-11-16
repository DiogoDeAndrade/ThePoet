import urllib.request
import urllib.parse
from html.parser import HTMLParser

class MyHTMLParser(HTMLParser):
    def __init__(self):
        HTMLParser.__init__(self)

        self.hCount = 0
        self.validWord = -1
        self.wordsFound = []

    def get_attr(self, attrs, name):
        for v in attrs:
            if (v[0] == name):
                return v[1]
        return ""

    def handle_starttag(self, tag, attrs):
        self.hCount = self.hCount + 1
        if (tag == "div"):
            # Check if this is the content            
            if (self.get_attr(attrs, "class") == "w"):
                self.validWord = self.hCount

    def handle_endtag(self, tag):
        self.hCount = self.hCount- 1
        if (self.hCount != self.validWord):
            self.validWord = -1

    def handle_data(self, data):
        if (self.validWord == self.hCount):
            self.wordsFound.append(data)

def get_rhymes(lang, word, retry = True):
    url = "https://www.rhymit.com/pt/palavras-que-rimam-com-" + urllib.parse.quote(word)
    try:
        page = urllib.request.urlopen(url)
        page = page.read().decode("utf8")

        parser = MyHTMLParser()
        parser.feed(page)

        return parser.wordsFound
    except urllib.error.HTTPError:
        print("Can't retrieve " + word)
        return []
    except urllib.error.TimeoutError:
        if (retry):
            print("Timeout retrieving " + word + ", retrying...")
            return get_rhymes(lang, word, False)
        else:
            print("Timeout retrieving " + word)
            return []

#print(get_rhymes("pt", "chão"))
