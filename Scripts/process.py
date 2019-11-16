import pyphen
import time
from get_rhyme import get_rhymes

def remove_all(str, c):
    while (True):
        idx = str.find(c)
        if (idx == -1):
            break
        str = str[:idx] + str[idx+1:]

    return str

def remove_punctuation(str):
    str = remove_all(str, '.')
    str = remove_all(str, ',')
    str = remove_all(str, '!')
    str = remove_all(str, '?')
    str = remove_all(str, '-')
    str = remove_all(str, '...')

    return str

language = "pt_PT"
source_file = "cancioneiro.txt"

rhymes = {}

dic = pyphen.Pyphen(lang=language)

file = open(source_file, "r", encoding="utf-8")
if (file.mode != "r"):
    print("Failed to open file!")
    exit()

lines = file.readlines()

output_file = open("phrases_" + language + ".txt", "wt")

phrases = {}

for l in lines:
    l = l.strip()
    
    l = remove_all(l, " ...")
    l = remove_all(l, " ?")
    l = remove_all(l, " !")
    l = remove_all(l, ";")
    l = remove_all(l, ":")

    if (l == ""):
        continue
    if (l in phrases):
        continue
    if (l.count(" ") == 0):
        continue
    phrases[l] = True

    # Contar sílabas
    words = l.split(' ')
    hyphen_text = ""
    last_word = ""
    last_syl = ""
    sil_count = 0
    for w in words:
        h = dic.inserted(w)
        if (hyphen_text != ""):
            hyphen_text = hyphen_text + " "
        hyphen_text = hyphen_text + h
        split_text = h.split('-')
        sil_count = sil_count + len(split_text)
        last_word = w
        last_syl = split_text[-1]

    last_syl = remove_punctuation(last_syl)
    last_word = remove_punctuation(last_word)

    out_text = "(\""
    out_text = out_text + hyphen_text + "\", "
    out_text = out_text + "\"" + last_syl + "\", "
    out_text = out_text + "\"" + last_word + "\", "
    out_text = out_text + str(sil_count)
    out_text = out_text + ")"
    out_text = out_text + l + "\n"

    output_file.write(out_text)

    rhymes[last_word] = []

output_file.close()
file.close()

rhyme_lang = "pt"
if (language == "pt_PT"):
    rhyme_lang = "pt"

count = 0
for word in rhymes:

    if (rhymes[word] == []):
        print("Retrieving " + word, flush = True)

        r = get_rhymes(rhyme_lang, word)

        # Add only rhymes that can happen
        for tmp in r:
            if (tmp in rhymes):
                rhymes[word].append(tmp)


        time.sleep(1)
        #count = count + 1
        #if (count > 1):
        #    break


# Custom stuff, because this system is flawed
if (rhyme_lang == "pt"):
    rhymes['é'].append('pé')
    
    # Add the word itself to the list
    for word in rhymes:
        if (not word in rhymes[word]):
            rhymes[word].append(word)

    # Add reverse search (if a/b rhymes, b/a also rhymes)
    for word in rhymes:
        for rword in rhymes[word]:
            if (not rword in rhymes):
                rhymes[rword] = [ word ]
            else:
                if (not word in rhymes[rword]):
                    rhymes[rword].append(word)

output_file = open("rhymes_" + language + ".txt", "wt")

for word in rhymes:
    output_file.write(word + ":" + str(rhymes[word]) + "\n")

output_file.close()
