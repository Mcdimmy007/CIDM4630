import streamlit as st
import pandas as pd
import sqlite3

# Connect to the database
conn = sqlite3.connect("scholarship.db")
df = pd.read_sql("SELECT * FROM StudentGrades", conn)

# Scholarship logic
def calculate_award_detail(row):
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
    social_course = max((c for c in REQUIRED_SOCIAL if c in valid_courses), key=lambda c: valid_courses[c], default=None)

    result = {
        "Mandatory Courses": {},
        "Optional Courses": {},
        "Average": None,
        "Scholarship Result": None
    }

    if not lang_course:
        result["Scholarship Result"] = "❌ Ineligible - No valid English/French course"
        return result
    if not social_course:
        result["Scholarship Result"] = "❌ Ineligible - No valid Social Studies course"
        return result

    result["Mandatory Courses"] = {
        lang_course: valid_courses[lang_course],
        social_course: valid_courses[social_course]
    }

    remaining_courses = {k: v for k, v in valid_courses.items() if k not in {lang_course, social_course}}
    top_remaining = sorted(remaining_courses.items(), key=lambda x: x[1], reverse=True)[:3]

    if len(top_remaining) < 3:
        result["Scholarship Result"] = "❌ Ineligible - Fewer than 5 eligible courses"
        return result

    result["Optional Courses"] = dict(top_remaining)
    marks = [valid_courses[lang_course], valid_courses[social_course]] + [v for _, v in top_remaining]
    avg = sum(marks) / 5
    result["Average"] = round(avg, 2)

    if avg >= 80:
        result["Scholarship Result"] = f"✅ Eligible for $2500 - Average: {avg:.2f}"
    elif avg >= 75:
        result["Scholarship Result"] = f"✅ Eligible for $1500 - Average: {avg:.2f}"
    else:
        result["Scholarship Result"] = f"❌ Ineligible - Average: {avg:.2f}"

    return result

# Title
st.title("🎓 Welcome to Rutherford Scholarship Checker")

# Display student names only
st.subheader("📋 List of Students")
st.write(", ".join(sorted(df["StudentName"].unique())))

# Sidebar filters
st.sidebar.title("Filters")
filter_mode = st.sidebar.radio("Filter by:", ["All", "Grade Level", "Select Names"])

filtered_df = pd.DataFrame()
if filter_mode == "All":
    filtered_df = df.copy()

elif filter_mode == "Grade Level":
    grade_choice = st.sidebar.selectbox("Choose Grade:", sorted(df["GradeLevel"].unique()))
    filtered_df = df[df["GradeLevel"] == grade_choice]

elif filter_mode == "Select Names":
    names_selected = st.sidebar.multiselect("Select Student(s):", sorted(df["StudentName"].unique()))
    if names_selected:
        filtered_df = df[df["StudentName"].isin(names_selected)]

if not filtered_df.empty:
    st.subheader("🎓 Scholarship Eligibility Results")
    detailed_results = []
    for _, row in filtered_df.iterrows():
        detail = calculate_award_detail(row)
        mandatory = ", ".join([f"{k}: {v}" for k, v in detail["Mandatory Courses"].items()])
        optional = ", ".join([f"{k}: {v}" for k, v in detail["Optional Courses"].items()])
        detailed_results.append([
            row["StudentName"],
            row["GradeLevel"],
            detail["Average"] if detail["Average"] else "-",
            mandatory,
            optional,
            detail["Scholarship Result"]
        ])

    st.dataframe(pd.DataFrame(detailed_results, columns=[
        "Student Name", "Grade Level", "Average", "Mandatory Courses", "Optional Courses", "Scholarship Result"
    ]))

conn.close()
