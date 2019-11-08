import pyphen

language = "pt_PT"
source_file = "cancioneiro.txt"

dic = pyphen.Pyphen(lang=language)

file = open(source_file, "r")
if (file.mode != "r"):
    print("Failed to open file!")
    exit()

lines = file.readlines()

output_file = open("phrases_" + language + ".txt", "wt")

phrases = {}

for l in lines:
    l = l.strip()
    
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
    sil_count = 0
    for w in words:
        h = dic.inserted(w)
        if (hyphen_text != ""):
            hyphen_text = hyphen_text + " "
        hyphen_text = hyphen_text + h
        split_text = h.split('-')
        sil_count = sil_count + len(split_text)

    out_text = "(\""
    out_text = out_text + hyphen_text + "\", "
    out_text = out_text + str(sil_count)
    out_text = out_text + ")"
    out_text = out_text + l + "\n"

    output_file.write(out_text)

output_file.close()
file.close()