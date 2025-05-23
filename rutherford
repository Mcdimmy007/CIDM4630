import sqlite3
import pandas as pd
from tabulate import tabulate

# ----------------------------------------
# STEP 1: Setup / Reset the Database
# ----------------------------------------

conn = sqlite3.connect("scholarship.db")
cursor = conn.cursor()

cursor.execute("DROP TABLE IF EXISTS StudentGrades")

cursor.execute('''
CREATE TABLE StudentGrades (
    StudentID INTEGER PRIMARY KEY AUTOINCREMENT,
    StudentName TEXT,
    GradeLevel TEXT,
    "English 30-1" INTEGER,
    "Social Studies 30-1" INTEGER,
    "Biology 30" INTEGER,
    "Mathematics 30-1" INTEGER,
    "Chemistry 30" INTEGER,
    "Art 30" INTEGER
)
''')

students = [
    ("Alice", "Grade 10", 90, 85, 88, 91, 87, 80),
    ("Ben", "Grade 10", 82, 78, 85, 89, 84, 70),
    ("Carla", "Grade 9", 85, 80, 80, 80, 80, 80),
    ("Daniel", "Grade 9", 76, 75, 78, 80, 82, 79),
    ("Eliza", "Grade 8", 79, 77, 75, 80, 83, 76),
    ("Farah", "Grade 10", 80, 81, 78, 79, 77, 80),
    ("George", "Grade 10", 90, 89, 85, 88, 87, 90),
    ("Hannah", "Grade 9", None, 85, 88, 91, 87, 80),
    ("Isaac", "Grade 9", 82, None, 85, 89, 84, 70),
    ("Jade", "Grade 8", 85, 80, None, None, None, None),
    ("Kevin", "Grade 10", 65, 67, 68, 70, 71, 72),
    ("Lena", "Grade 8", 50, 45, 60, 55, 40, 65),
    ("Milo", "Grade 9", None, None, 80, 80, 80, 80),
    ("Nina", "Grade 10", 90, 90, None, None, None, None),
    ("Omar", "Grade 10", 70, 70, 72, 68, 65, 60),
    ("Pia", "Grade 8", None, None, None, None, None, None),
    ("Quinn", "Grade 9", 85, None, None, None, None, None),
    ("Ravi", "Grade 9", None, 85, None, None, None, None),
    ("Sara", "Grade 10", 74, 74, 74, 74, 74, 74),
    ("Toby", "Grade 10", 80, 70, 70, 70, 70, 70),
    ("Uma", "Grade 10", 70, 75, 70, 75, 70, 75)
]

cursor.executemany('''
    INSERT INTO StudentGrades (
        StudentName, GradeLevel,
        "English 30-1", "Social Studies 30-1", 
        "Biology 30", "Mathematics 30-1", "Chemistry 30", "Art 30"
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?)
''', students)

conn.commit()

# ----------------------------------------
# STEP 2: Scholarship Eligibility Logic
# ----------------------------------------
def calculate_award(row):
    REQUIRED_LANGUAGES = {"English 30-1", "English 30-2", "Franais 30-1", "Franais 30-2"}
    REQUIRED_SOCIAL = {"Social Studies 30-1", "Social Studies 30-2"}
    ELIGIBLE_COURSES = REQUIRED_LANGUAGES | REQUIRED_SOCIAL | {
        "Biology 30", "Chemistry 30", "Physics 30", "Science 30",
        "Mathematics 30-1", "Mathematics 30-2", "Mathematics 31",
        "Aboriginal Studies 30", "Language and Culture 30",
        "Art 30", "Drama 30", "Music 30", "Dance 30",
        "Physical Education 30", "Career and Life Management"
    }

    valid_courses = {col: row[col] for col in ELIGIBLE_COURSES if col in row and pd.notna(row[col])}
    lang_course = max((c for c in REQUIRED_LANGUAGES if c in valid_courses), key=lambda c: valid_courses[c], default=None)
    if not lang_course:
        return "❌ Ineligible - No valid English/French course"
    social_course = max((c for c in REQUIRED_SOCIAL if c in valid_courses), key=lambda c: valid_courses[c], default=None)
    if not social_course:
        return "❌ Ineligible - No valid Social Studies course"

    remaining_courses = {k: v for k, v in valid_courses.items() if k not in {lang_course, social_course}}
    top_remaining = sorted(remaining_courses.values(), reverse=True)[:3]

    if len(top_remaining) < 3:
        return "❌ Ineligible - Fewer than 5 eligible courses"

    marks = [valid_courses[lang_course], valid_courses[social_course]] + top_remaining
    avg = sum(marks) / 5

    if avg >= 80:
        return f"✅ Eligible for $2500 - Average: {avg:.2f}"
    elif avg >= 75:
        return f"✅ Eligible for $1500 - Average: {avg:.2f}"
    else:
        return f"❌ Ineligible - Average: {avg:.2f}"

# ----------------------------------------
# STEP 3: Main Loop Interface
# ----------------------------------------
while True:
    print("\n📘 RUTHERFORD SCHOLARSHIP CHECKER")
    print("1. Check all students or by name")
    print("2. Search students by Grade Level")
    print("3. Exit")

    mode = input("\nEnter 1, 2, or 3 to choose an option: ").strip()

    if mode == "3" or mode.lower() == "exit":
        print("👋 Exiting Scholarship Checker.")
        break

    elif mode == "1":
        cursor = conn.cursor()
        cursor.execute("SELECT DISTINCT StudentName FROM StudentGrades ORDER BY StudentName")
        all_students = [row[0] for row in cursor.fetchall()]
        print("\n📋 Available Students:")
        print(", ".join(all_students))

        submode = input("\nType 'all' to check everyone, or enter specific names (comma-separated): ").strip()
        requested_names = all_students if submode.lower() == "all" else [name.strip() for name in submode.split(",") if name.strip()]

        all_results = []
        for name in requested_names:
            df = pd.read_sql("SELECT * FROM StudentGrades WHERE LOWER(StudentName) = LOWER(?)", conn, params=(name,))
            if df.empty:
                all_results.append([f"{name:<20}", "-", "❌ Not found in database"])
            else:
                df["Scholarship Result"] = df.apply(calculate_award, axis=1)
                for _, row in df.iterrows():
                    all_results.append([f"{row['StudentName']:<20}", f"{row['GradeLevel']:<10}", row["Scholarship Result"]])

        print("\n🎓 Scholarship Eligibility Summary:")
        print(tabulate(all_results, headers=["Student Name", "Grade Level", "Scholarship Result"], tablefmt="grid"))

        save = input("\nWould you like to export the results? (yes/no): ").strip().lower()
        if save == "yes":
            filename = input("Enter filename (without extension): ").strip()
            export_type = input("Type 'csv' or 'excel': ").strip().lower()
            df_export = pd.DataFrame(all_results, columns=["Student Name", "Grade Level", "Scholarship Result"])
            if export_type == "csv":
                df_export.to_csv(f"{filename}.csv", index=False)
                print(f"✅ Results exported to {filename}.csv")
            elif export_type == "excel":
                df_export.to_excel(f"{filename}.xlsx", index=False)
                print(f"✅ Results exported to {filename}.xlsx")
            else:
                print("❌ Invalid export type. No file saved.")

    elif mode == "2":
        grade = input("Enter grade level (e.g., Grade 9, Grade 10): ").strip()
        df = pd.read_sql("SELECT * FROM StudentGrades WHERE GradeLevel = ?", conn, params=(grade,))
        if df.empty:
            print(f"\n❌ No students found in {grade}.")
        else:
            df["Scholarship Result"] = df.apply(calculate_award, axis=1)
            output = df[["StudentName", "GradeLevel", "Scholarship Result"]]
            print(f"\n🎓 Scholarship Results for {grade}:")
            print(tabulate(output.values.tolist(), headers=output.columns, tablefmt="grid"))

            save = input("\nWould you like to export the results? (yes/no): ").strip().lower()
            if save == "yes":
                filename = input("Enter filename (without extension): ").strip()
                export_type = input("Type 'csv' or 'excel': ").strip().lower()
                if export_type == "csv":
                    output.to_csv(f"{filename}.csv", index=False)
                    print(f"✅ Results exported to {filename}.csv")
                elif export_type == "excel":
                    output.to_excel(f"{filename}.xlsx", index=False)
                    print(f"✅ Results exported to {filename}.xlsx")
                else:
                    print("❌ Invalid export type. No file saved.")
    else:
        print("❌ Invalid option. Please choose 1, 2, or 3.")

conn.close()
